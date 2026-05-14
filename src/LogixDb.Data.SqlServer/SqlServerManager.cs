using System.Data;
using System.Diagnostics.CodeAnalysis;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using LogixDb.Data.Abstractions;
using LogixDb.Migrations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Dapper;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Represents a SQL Server database implementation of the ILogixDbManager interface.
/// Provides functionality for database creation, migration, Target management,
/// and dropping operations.
/// </summary>
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public sealed class SqlServerManager(DbConnectionInfo connectionInfo) : IDbManager
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
        await using var connection = await OpenConnection(token);

        return await connection.QueryAsync<Target>(
            SqlServerScript.ListTargets,
            new { TargetKey = targetKey }
        );
    }

    /// <inheritdoc />
    public async Task<Target> GetTarget(string targetKey, int version = 0, CancellationToken token = default)
    {
        await using var connection = await OpenConnection(token);

        var script = version > 0 ? SqlServerScript.GetTargetByVersion : SqlServerScript.GetTargetByLatest;
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
        await RestoreTargetVersionAsync(target, token);
    }

    /// <inheritdoc />
    public Task DeleteTarget(string targetKey, CancellationToken token = default)
    {
        return ExecuteSqlScriptAsync(
            SqlServerScript.DeleteTarget,
            new { TargetKey = targetKey },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteVersion(string targetKey, int versionNumber, CancellationToken token = default)
    {
        return ExecuteSqlScriptAsync(
            SqlServerScript.DeleteVersion,
            new { TargetKey = targetKey },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteVersions(string targetKey, int beforeVersion, CancellationToken token = default)
    {
        return ExecuteSqlScriptAsync(
            SqlServerScript.DeleteVersionsByNumber,
            new { TargetKey = targetKey, VersionNumber = beforeVersion },
            token
        );
    }

    /// <inheritdoc />
    public Task DeleteVersions(string targetKey, DateTime beforeDate, CancellationToken token = default)
    {
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
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
            throw;
        }
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
    /// <param name="target">The target object containing information about the version to restore.</param>
    /// <param name="token">A cancellation token to observe while the operation is performed.</param>
    /// <returns>A task that represents the asynchronous restoration operation.</returns>
    private async Task RestoreTargetVersionAsync(Target target, CancellationToken token)
    {
        await using var connection = await OpenConnection(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        try
        {
            // 1. Get the table names from the schema to determine which component to import
            var tableNames = await connection.QueryAsync<string>(
                SqlServerScript.GetTableNames,
                transaction: transaction
            );

            // 2. Compile AND Materialize the data tables.
            // Calling .ToList() here is critical: it forces L5Sharp to parse all data
            // and maps them to DataTables before we start the bulk copy process, preventing rollback exceptions.
            var dataTables = target.Compile(tableNames.ToArray()).ToList();

            // 3. Perform a bulk write of all compiled data to the database.
            var writer = new SqlServerWriter(target.VersionId, connection, (SqlTransaction)transaction);
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