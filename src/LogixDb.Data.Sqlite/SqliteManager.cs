using System.Data;
using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using LogixDb.Data.Abstractions;
using LogixDb.Migrations;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Manages SQLite database operations such as migrations, connections, and database removal.
/// </summary>
/// <remarks>
/// This class is a concrete implementation of the <c>ILogixDbManager</c> interface, designed to
/// work with SQLite databases. It facilitates operations like running database migrations,
/// connecting to the database, and managing the database file lifecycle.
/// </remarks>
public sealed class SqliteManager : IDbManager
{
    /// <summary>
    /// Stores the connection information required to configure and interact with an SQLite database.
    /// This includes details such as the database provider, source file, and optional parameters
    /// like user credentials, encryption settings, and connection port.
    /// </summary>
    private readonly DbConnectionInfo _connectionInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqliteManager"/> class with the specified database connection information.
    /// </summary>
    /// <param name="connectionInfo">The connection information used to configure and connect to the SQLite database.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="connectionInfo"/> is null.</exception>
    public SqliteManager(DbConnectionInfo connectionInfo)
    {
        _connectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof(connectionInfo));
        ConfigureSqlite();
    }

    /// <inheritdoc />
    public async Task Migrate(ComponentOptions options = ComponentOptions.All,
        CancellationToken token = default)
    {
        await using var provider = BuildMigrationProvider(_connectionInfo.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        await ConfigureDatabase(token);
    }

    /// <inheritdoc />
    public async Task Migrate(long version, ComponentOptions options = ComponentOptions.All,
        CancellationToken token = default)
    {
        await using var provider = BuildMigrationProvider(_connectionInfo.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp(version);
        await ConfigureDatabase(token);
    }

    /// <inheritdoc />
    public Task Drop(CancellationToken token = default)
    {
        if (File.Exists(_connectionInfo.Source))
        {
            File.Delete(_connectionInfo.Source);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IDbConnection> Connect(CancellationToken token = default)
    {
        if (!File.Exists(_connectionInfo.Source))
            throw new FileNotFoundException($"Database file not found: {_connectionInfo.Source}");

        var connection = new SqliteConnection(_connectionInfo.ToConnectionString());
        await connection.OpenAsync(token);
        return connection;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Target>> ListTargets(string? targetKey = null, CancellationToken token = default)
    {
        await using var connection = await OpenConnection(token);
        return await connection.QueryAsync<Target>(SqliteScript.ListTargets, new { TargetKey = targetKey });
    }

    /// <inheritdoc />
    public async Task<Target> GetTarget(string targetKey, int version = 0, CancellationToken token = default)
    {
        await using var connection = await OpenConnection(token);

        var script = version > 0 ? SqliteScript.GetTargetByVersion : SqliteScript.GetTargetByLatest;
        var parameters = new { TargetKey = targetKey, VersionNumber = version };
        var result = await connection.QuerySingleOrDefaultAsync<Target>(script, parameters);

        if (result is null)
            throw new InvalidOperationException($"Target '{targetKey}' version {version} not found in the database.");

        return result;
    }

    /// <inheritdoc />
    public async Task ImportTarget(Target target, CancellationToken token = default)
    {
        await PostTargetVersionAsync(target, token);
        await MergeTargetDataAsync(target, token);
    }

    /// <inheritdoc />
    public Task DeleteTarget(string targetKey, CancellationToken token = default)
    {
        return ExecuteSqliteScriptAsync(
            SqliteScript.DeleteTarget,
            new { TargetKey = targetKey },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteVersion(string targetKey, int versionNumber, CancellationToken token = default)
    {
        return ExecuteSqliteScriptAsync(
            SqliteScript.DeleteVersion,
            new { TargetKey = targetKey, VersionNumber = versionNumber },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteVersions(string targetKey, int beforeVersion, CancellationToken token = default)
    {
        return ExecuteSqliteScriptAsync(
            SqliteScript.DeleteVersionsByNumber,
            new { TargetKey = targetKey, VersionNumber = beforeVersion },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteVersions(string targetKey, DateTime beforeDate, CancellationToken token = default)
    {
        return ExecuteSqliteScriptAsync(
            SqliteScript.DeleteVersionsBeforeDate,
            new { TargetKey = targetKey, BeforeDate = beforeDate },
            token
        );
    }

    /// <summary>
    /// Executes the specified SQL script asynchronously using the database session.
    /// </summary>
    private async Task ExecuteSqliteScriptAsync(string scriptName, object parameters, CancellationToken token)
    {
        await using var connection = await OpenConnection(token);
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
    /// Posts a new target version to the database and stores the associated metadata for this version.
    /// </summary>
    private async Task PostTargetVersionAsync(Target target, CancellationToken token)
    {
        await using var connection = await OpenConnection(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        try
        {
            // Insert target key if not already.
            await connection.ExecuteAsync(SqliteScript.PostTarget, target, transaction);

            // Inserts a new version for the target key (handles getting target id in scripts) and returns the inserted version id.
            // This is needed in some spots, and is an indicator that the version was posted. 
            target.VersionId = await connection.ExecuteScalarAsync<int>(SqliteScript.PostVersion, target, transaction);

            // Inserts all the configured metadata for the version.
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
    private async Task MergeTargetDataAsync(Target target, CancellationToken token)
    {
        await using var connection = await OpenConnection(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        try
        {
            // 1. Get the table names from the schema to determine which component to import
            var tableNames = await connection.QueryAsync<string>(
                SqliteScript.GetTableNames,
                transaction: transaction
            );

            // 2. Compile AND Materialize the data tables.
            // Calling .ToList() here is critical: it forces L5Sharp to parse all data
            // and maps them to DataTables before we start the bulk copy process, preventing rollback exceptions.
            var dataTables = target.Compile(tableNames.ToArray()).ToList();

            // 3. Perform a bulk write of all compiled data to the database.
            var writer = new SqliteWriter(target.VersionId, connection, (SqliteTransaction)transaction);
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
        var connection = new SqliteConnection(_connectionInfo.ToConnectionString());
        await connection.OpenAsync(token);
        return connection;
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
        await using var connection = new SqliteConnection(_connectionInfo.ToConnectionString());
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