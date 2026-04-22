using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using LogixDb.Data.Abstractions;
using LogixDb.Migrations;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Manages SQLite database operations such as migrations, connections, and database removal.
/// </summary>
/// <remarks>
/// This class is a concrete implementation of the <c>ILogixDbManager</c> interface, designed to
/// work with SQLite databases. It facilitates operations like running database migrations,
/// connecting to the database, and managing the database file lifecycle.
/// </remarks>
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public sealed class SqliteManager(DbConnectionInfo connectionInfo, ILogger logger) : IDbManager
{
    /// <inheritdoc />
    public async Task Migrate(ComponentOptions options = ComponentOptions.All,
        CancellationToken token = default)
    {
        await using var provider = BuildMigrationProvider(connectionInfo.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        await ConfigureDatabase(token);
    }

    /// <inheritdoc />
    public async Task Migrate(long version, ComponentOptions options = ComponentOptions.All,
        CancellationToken token = default)
    {
        await using var provider = BuildMigrationProvider(connectionInfo.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp(version);
        await ConfigureDatabase(token);
    }

    /// <inheritdoc />
    public Task Drop(CancellationToken token = default)
    {
        if (File.Exists(connectionInfo.Source))
        {
            File.Delete(connectionInfo.Source);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IDbConnection> Connect(CancellationToken token = default)
    {
        if (!File.Exists(connectionInfo.Source))
            throw new FileNotFoundException($"Database file not found: {connectionInfo.Source}");

        var connection = new SqliteConnection(connectionInfo.ToConnectionString());
        await connection.OpenAsync(token);
        return connection;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Target>> ListTargets(string? targetKey = null, CancellationToken token = default)
    {
        logger.LogInformation("Listing targets {TargetKey}", targetKey ?? "all");
        
        ConfigureSqlite();
        await using var connection = await OpenConnection(token);
        return await connection.QueryAsync<Target>(SqliteScript.ListTargets, new { TargetKey = targetKey });
    }

    /// <inheritdoc />
    public Task<Target?> GetTarget(string targetKey, int version = 0, CancellationToken token = default)
    {
        logger.LogInformation("Getting target {TargetKey} (version: {Version})", targetKey, version);
        return GetTargetAsync(targetKey, version, token);
    }

    /// <inheritdoc />
    public Task PostTarget(Target target, CancellationToken token = default)
    {
        logger.LogInformation("Posting new version for target {TargetKey}", target.TargetKey);
        return PostTargetVersionAsync(target, token);
    }

    /// <inheritdoc />
    public async Task ImportTarget(Target target, CancellationToken token = default)
    {
        logger.LogInformation("Importing target {TargetKey}", target.TargetKey);

        await PostTargetVersionAsync(target, token);
        await ExecuteSqliteScriptAsync(SqliteScript.DeleteTargetInstances, new { target.TargetKey }, token);
        await RestoreTargetVersionAsync(target, token);
    }

    /// <inheritdoc />
    public async Task RestoreTarget(string targetKey, int version = 0, CancellationToken token = default)
    {
        logger.LogInformation("Restoring target {TargetKey} (version: {Version})", targetKey, version);

        var target = await GetTargetAsync(targetKey, version, token);

        if (target is null)
            throw new InvalidOperationException($"Target '{targetKey}' with version {version} not found");

        if (target.InstanceId == 0)
            await RestoreTargetVersionAsync(target, token);
    }

    /// <inheritdoc />
    public async Task ArchiveTarget(string targetKey, int version = 0, CancellationToken token = default)
    {
        logger.LogInformation("Archiving target {TargetKey} (version: {Version})", targetKey, version);

        var target = await GetTargetAsync(targetKey, version, token);

        if (target?.InstanceId > 0)
            await ExecuteSqliteScriptAsync(SqliteScript.DeleteVersionInstance, new { target.InstanceId }, token);
    }

    /// <inheritdoc />
    public Task PruneTarget(string targetKey, CancellationToken token = default)
    {
        logger.LogInformation("Pruning instances for target {TargetKey}", targetKey);

        return ExecuteSqliteScriptAsync(
            SqliteScript.DeleteTargetInstances,
            new { TargetKey = targetKey },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteTarget(string targetKey, CancellationToken token = default)
    {
        logger.LogInformation("Deleting target {TargetKey}", targetKey);

        return ExecuteSqliteScriptAsync(
            SqliteScript.DeleteTarget,
            new { TargetKey = targetKey },
            token
        );
    }

    /// <inheritdoc />
    public Task TruncateTarget(string targetKey, int beforeVersion, CancellationToken token = default)
    {
        logger.LogInformation("Truncating versions for target {TargetKey} before version {Version}", targetKey,
            beforeVersion);

        return ExecuteSqliteScriptAsync(
            SqliteScript.DeleteVersionsByNumber,
            new { TargetKey = targetKey, VersionNumber = beforeVersion },
            token
        );
    }

    /// <inheritdoc />
    public Task TruncateTarget(string targetKey, DateTime beforeDate, CancellationToken token = default)
    {
        logger.LogInformation("Truncating versions for target {TargetKey} before {Date}", targetKey, beforeDate);

        return ExecuteSqliteScriptAsync(
            SqliteScript.DeleteVersionsBeforeDate,
            new { TargetKey = targetKey, BeforeDate = beforeDate },
            token
        );
    }

    /// <summary>
    /// Configures SQLite-specific settings for Dapper by modifying type mappings and adding a custom type handler for GUIDs.
    /// Removes existing type maps for <see cref="Guid"/> and nullable <see cref="Guid"/> types
    /// and registers a custom <see cref="SqliteGuidHandler"/> to handle GUID serialization and deserialization.
    /// </summary>
    private static void ConfigureSqlite()
    {
        SqlMapper.RemoveTypeMap(typeof(Guid));
        SqlMapper.RemoveTypeMap(typeof(Guid?));
        SqlMapper.AddTypeHandler(new SqliteGuidHandler());
    }

    /// <summary>
    /// Executes the specified SQL script asynchronously using the database session.
    /// </summary>
    private async Task ExecuteSqliteScriptAsync(string scriptName, object parameters, CancellationToken token)
    {
        await using var connection = (SqliteConnection)await Connect(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        try
        {
            await connection.ExecuteAsync(scriptName, parameters, transaction);
            await transaction.CommitAsync(token);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a target from the database based on the specified key and version.
    /// </summary>
    private async Task<Target?> GetTargetAsync(string targetKey, int version, CancellationToken token)
    {
        ConfigureSqlite();
        await using var dbConnection = (SqliteConnection)await Connect(token);

        if (version > 0)
        {
            return await dbConnection.QuerySingleOrDefaultAsync<Target>(
                SqliteScript.GetTargetByVersion,
                new { TargetKey = targetKey, VersionNumber = version }
            );
        }

        return await dbConnection.QuerySingleOrDefaultAsync<Target>(
            SqliteScript.GetTargetByLatest,
            new { TargetKey = targetKey }
        );
    }

    /// <summary>
    /// Posts a new target version to the database and stores the associated metadata for this version.
    /// </summary>
    private async Task PostTargetVersionAsync(Target target, CancellationToken token)
    {
        await using var connection = (SqliteConnection)await Connect(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        try
        {
            await connection.ExecuteAsync(SqliteScript.PostTarget, target, transaction);

            await connection.ExecuteAsync(SqliteScript.PostInfo,
                target.Info.Select(p => new
                {
                    property_id = Guid.NewGuid(),
                    version_id = target.VersionId,
                    property_name = p.Key,
                    property_value = p.Value
                }).ToList(),
                transaction
            );

            await transaction.CommitAsync(token);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    /// <summary>
    /// Restores a specific version of the target by creating a new entry in the database and populating
    /// associated tables with the target's data.
    /// </summary>
    private async Task RestoreTargetVersionAsync(Target target, CancellationToken token)
    {
        await using var connection = (SqliteConnection)await Connect(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        try
        {
            target.InstanceId = await connection.ExecuteScalarAsync<int>(
                SqliteScript.PostInstance,
                new { target.VersionId, ResotredOn = DateTime.Now, RestoredBy = Environment.UserName },
                transaction
            );

            var tableNames =
                (await connection.QueryAsync<string>(SqliteScript.GetComponentTables, transaction: transaction))
                .ToArray();
            var dataTables = target.Compile(tableNames.ToArray());

            var writer = new SqliteWriter(connection, (SqliteTransaction)transaction);
            await writer.WriteAsync(dataTables, token);

            await transaction.CommitAsync(token);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    /// <summary>
    /// Opens a new SQLite database connection using the provided connection information.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the opened <see cref="SqliteConnection"/> instance.</returns>
    private async Task<SqliteConnection> OpenConnection(CancellationToken token)
    {
        var connection = new SqliteConnection(connectionInfo.ToConnectionString());
        await connection.OpenAsync(token);
        return connection;
    }

    /// <summary>
    /// Creates a migration provider for managing database migrations using FluentMigrator.
    /// Configures the service collection with the necessary settings for SQLite integration,
    /// including connection string, version table metadata, and migration scanning.
    /// </summary>
    /// <param name="connectionString">The connection string for the SQLite database.</param>
    /// <param name="options"></param>
    /// <returns>A configured <see cref="ServiceProvider"/> instance to execute migrations.</returns>
    private static ServiceProvider BuildMigrationProvider(string connectionString, ComponentOptions options)
    {
        var services = new ServiceCollection();

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(connectionString)
                .WithVersionTable(new MigrationTableMetaData())
                .ScanIn(
                    typeof(MigrationTableMetaData).Assembly,
                    typeof(SqliteManager).Assembly)
                .For.Migrations()
            );

        var tags = MigrationTag.GetTags(options);
        services.Configure<RunnerOptions>(opt => opt.Tags = tags.ToArray());
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Configures SQLite performance-related PRAGMA settings to enhance database operations.
    /// This method is responsible for setting various SQLite pragmas, such as journal mode,
    /// auto vacuum mode, synchronization settings, and memory usage configurations, 
    /// to optimize the database performance.
    /// </summary>
    /// <remarks>
    /// The method adjusts settings including WAL (Write-Ahead Logging) for journaling,
    /// incremental vacuum, and memory configurations such as mmap size and cache size.
    /// These settings are tailored to improve performance while maintaining data integrity.
    /// </remarks>
    private async Task ConfigureDatabase(CancellationToken token)
    {
        await using var connection = new SqliteConnection(connectionInfo.ToConnectionString());
        await connection.OpenAsync(token);

        await connection.ExecuteAsync(
            """
            PRAGMA journal_mode = WAL;
            PRAGMA auto_vacuum = FULL;
            PRAGMA temp_store = MEMORY;
            PRAGMA busy_timeout = 5000;
            PRAGMA page_size = 16384;
            """
        );
    }
}