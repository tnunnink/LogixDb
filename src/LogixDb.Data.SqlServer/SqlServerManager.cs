using System.Data;
using System.Diagnostics.CodeAnalysis;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using LogixDb.Data.Abstractions;
using LogixDb.Migrations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using Microsoft.Extensions.Logging;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Represents a SQL Server database implementation of the ILogixDbManager interface.
/// Provides functionality for database creation, migration, Target management,
/// and dropping operations.
/// </summary>
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public sealed class SqlServerManager(DbConnectionInfo connectionInfo, ILogger logger) : IDbManager
{
    /// <inheritdoc />
    public async Task Migrate(ComponentOptions options = ComponentOptions.All, CancellationToken token = default)
    {
        await EnsureCreated(token);
        await using var provider = BuildMigrationProvider(connectionInfo.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        await ConfigureDatabase(token);
    }

    /// <inheritdoc />
    public async Task Migrate(long version, ComponentOptions options = ComponentOptions.All,
        CancellationToken token = default)
    {
        await EnsureCreated(token);
        await using var provider = BuildMigrationProvider(connectionInfo.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp(version);
        await ConfigureDatabase(token);
    }

    /// <inheritdoc />
    public async Task Drop(CancellationToken token = default)
    {
        await using var masterConnection = await OpenMasterConnectionAsync(token);

        await masterConnection.ExecuteAsync(
            $"""
             IF EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName)
             BEGIN
               ALTER DATABASE [{connectionInfo.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
               DROP DATABASE [{connectionInfo.Database}]
             END
             """,
            new { DatabaseName = connectionInfo.Database }
        );
    }

    /// <inheritdoc />
    public async Task<IDbConnection> Connect(CancellationToken token = default)
    {
        var connection = new SqlConnection(connectionInfo.ToConnectionString());
        await connection.OpenAsync(token);
        return connection;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Target>> ListTargets(string? targetKey = null, CancellationToken token = default)
    {
        logger.LogInformation("Listing targets {TargetKey}", targetKey ?? "all");
        await using var connection = await OpenConnection(token);
        return await connection.QueryAsync<Target>(SqlServerScript.ListTargets, new { TargetKey = targetKey });
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
        await ExecuteSqlScriptAsync(SqlServerScript.DeleteTargetInstances, new { target.TargetKey }, token);
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
            await ExecuteSqlScriptAsync(SqlServerScript.DeleteVersionInstance, new { target.InstanceId }, token);
    }

    /// <inheritdoc />
    public Task PruneTarget(string targetKey, CancellationToken token = default)
    {
        logger.LogInformation("Pruning instances for target {TargetKey}", targetKey);

        return ExecuteSqlScriptAsync(
            SqlServerScript.DeleteTargetInstances,
            new { TargetKey = targetKey },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteTarget(string targetKey, CancellationToken token = default)
    {
        logger.LogInformation("Deleting target {TargetKey}", targetKey);

        return ExecuteSqlScriptAsync(
            SqlServerScript.DeleteTarget,
            new { TargetKey = targetKey },
            token
        );
    }

    /// <inheritdoc />
    public Task TruncateTarget(string targetKey, int beforeVersion, CancellationToken token = default)
    {
        logger.LogInformation("Truncating versions for target {TargetKey} before version {Version}", targetKey,
            beforeVersion);

        return ExecuteSqlScriptAsync(
            SqlServerScript.DeleteVersionsByNumber,
            new { TargetKey = targetKey, VersionNumber = beforeVersion },
            token
        );
    }

    /// <inheritdoc />
    public Task TruncateTarget(string targetKey, DateTime beforeDate, CancellationToken token = default)
    {
        logger.LogInformation("Truncating versions for target {TargetKey} before {Date}", targetKey, beforeDate);

        return ExecuteSqlScriptAsync(
            SqlServerScript.DeleteVersionsBeforeDate,
            new { TargetKey = targetKey, BeforeDate = beforeDate },
            token
        );
    }

    /// <summary>
    /// Executes the specified SQL script asynchronously using the database session.
    /// </summary>
    /// <param name="scriptName">The name of the SQL script to execute.</param>
    /// <param name="parameters">The parameters to be passed along with the SQL script.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ExecuteSqlScriptAsync(string scriptName, object parameters, CancellationToken token)
    {
        await using var connection = await OpenConnection(token);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(token);

        try
        {
            await connection.ExecuteAsync(scriptName, parameters, transaction);
            await transaction.CommitAsync(token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute script {ScriptName}", scriptName);
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a target from the database based on the specified key and version.
    /// </summary>
    /// <param name="targetKey">The unique key identifying the target.</param>
    /// <param name="version">The version of the target to retrieve. If not specified, the latest version will be retrieved.</param>
    /// <param name="token">The cancellation token to observe for operation cancellation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the requested target.</returns>
    private async Task<Target?> GetTargetAsync(string targetKey, int version, CancellationToken token)
    {
        await using var connection = await OpenConnection(token);

        if (version > 0)
        {
            return await connection.QuerySingleOrDefaultAsync<Target>(
                SqlServerScript.GetTargetByVersion,
                new { TargetKey = targetKey, VersionNumber = version }
            );
        }

        return await connection.QuerySingleOrDefaultAsync<Target>(
            SqlServerScript.GetTargetByLatest,
            new { TargetKey = targetKey }
        );
    }

    /// <summary>
    /// Posts a new target version to the database and stores the associated metadata for this version.
    /// </summary>
    /// <param name="target">The target object containing the version details and associated metadata to be recorded.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to observe the operation for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation of recording the target version and metadata to the database.</returns>
    private async Task PostTargetVersionAsync(Target target, CancellationToken token)
    {
        await using var connection = await OpenConnection(token);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(token);

        try
        {
            // Post a target version to the database.
            // This only ensures target entry and adds a new version to the target_version table.
            // This does not "instantiate" the target to relation tables.
            await connection.ExecuteAsync(SqlServerScript.PostTarget, target, transaction);

            await connection.ExecuteAsync(SqlServerScript.PostInfo,
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to post target version for {TargetKey}", target.TargetKey);
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    /// <summary>
    /// Restores a specific version of the target by creating a new entry in the database and populating
    /// associated tables with the target's data.
    /// </summary>
    /// <param name="target">The target object containing information about the version to restore.</param>
    /// <param name="token">A cancellation token to observe while the operation is performed.</param>
    /// <returns>A task that represents the asynchronous restoration operation.</returns>
    private async Task RestoreTargetVersionAsync(Target target, CancellationToken token)
    {
        await using var connection = await OpenConnection(token);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(token);

        try
        {
            target.InstanceId = await connection.ExecuteScalarAsync<int>(
                SqlServerScript.PostInstance,
                new { target.VersionId, RestoredOn = DateTime.Now, RestoredBy = Environment.UserName },
                transaction
            );

            var tableNames = (await connection.QueryAsync<string>(SqlServerScript.GetComponentTables)).ToArray();
            var dataTables = target.Compile(tableNames.ToArray());

            var writer = new SqlServerWriter(connection, transaction);
            await writer.WriteAsync(dataTables, token);

            await transaction.CommitAsync(token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to restore target version for {TargetKey}", target.TargetKey);
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    /// <summary>
    /// Ensures that the associated SQL Server database is created. If the database does not exist, it creates it.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the database creation operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task EnsureCreated(CancellationToken token)
    {
        await using var masterConnection = await OpenMasterConnectionAsync(token);

        await masterConnection.ExecuteAsync(
            $"""
             IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName)
             BEGIN
                 CREATE DATABASE [{connectionInfo.Database}]
             END
             """,
            new { DatabaseName = connectionInfo.Database }
        );
    }

    /// <summary>
    /// Establishes and opens a connection to the master database of the specified SQL Server instance.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>Returns a <see cref="SqlConnection"/> object representing the opened connection to the master database.</returns>
    private async Task<SqlConnection> OpenMasterConnectionAsync(CancellationToken token)
    {
        var connectionString = connectionInfo.ToConnectionString("master");
        var masterConnection = new SqlConnection(connectionString);
        await masterConnection.OpenAsync(token);
        return masterConnection;
    }

    /// <summary>
    /// Opens a new connection to the database using the provided connection information.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to observe cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IDbConnection"/> instance representing the open connection.</returns>
    private async Task<SqlConnection> OpenConnection(CancellationToken token)
    {
        var connection = new SqlConnection(connectionInfo.ToConnectionString());
        await connection.OpenAsync(token);
        return connection;
    }

    /// <summary>
    /// Creates a service provider configured for running FluentMigrator migrations.
    /// </summary>
    /// <returns>
    /// A <see cref="ServiceProvider"/> configured with migration-specific services that
    /// include SQL Server support and migration scanning in the relevant assemblies.
    /// </returns>
    private static ServiceProvider BuildMigrationProvider(string connectionString, ComponentOptions options)
    {
        var services = new ServiceCollection();

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .WithVersionTable(new MigrationTableMetaData())
                .ScanIn(
                    typeof(MigrationTableMetaData).Assembly,
                    typeof(SqlServerManager).Assembly)
                .For.Migrations()
            );

        var tags = MigrationTag.GetTags(options);
        services.Configure<RunnerOptions>(opt => opt.Tags = tags.ToArray());
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Configures the connected SQL Server database by setting specific database-level properties.
    /// </summary>
    /// <param name="token">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    private async Task ConfigureDatabase(CancellationToken token)
    {
        await using var dbConnection = (SqlConnection)await Connect(token);
        await dbConnection.ExecuteAsync("ALTER DATABASE CURRENT SET RECOVERY SIMPLE;");
    }
}