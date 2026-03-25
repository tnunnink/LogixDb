using System.Data;
using System.Data.Common;
using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Exceptions;
using LogixDb.Migrations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace LogixDb.Data.SqlServer;

/// <summary>
/// Represents a SQL Server database implementation of the ILogixDatabase interface.
/// Provides functionality for database creation, migration, snapshot management,
/// importing, exporting, and purging operations.
/// </summary>
public sealed class SqlServerDb(SqlConnectionInfo connection) : ILogixDb
{
    /// <summary>
    /// Encapsulates connection-specific information for interacting with a SQL Server database.
    /// This includes details such as data source, catalog, authentication credentials, and other
    /// configuration settings necessary for establishing and managing the database connection.
    /// </summary>
    private readonly SqlConnectionInfo _connection = connection ?? throw new ArgumentNullException(nameof(connection));

    /// <inheritdoc />
    public async Task Migrate(TableOptions? options = null, CancellationToken token = default)
    {
        await EnsureCreated(token);
        await using var provider = BuildMigrationProvider(_connection.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    /// <inheritdoc />
    public async Task Migrate(long version, TableOptions? options = null, CancellationToken token = default)
    {
        await EnsureCreated(token);
        await using var provider = BuildMigrationProvider(_connection.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    /// <inheritdoc />
    public async Task Drop(CancellationToken token = default)
    {
        await using var connection = await OpenMasterConnectionAsync(token);

        await connection.ExecuteAsync(
            $"""
             IF EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName)
             BEGIN
               ALTER DATABASE [{_connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
               DROP DATABASE [{_connection.Database}]
             END
             """,
            new { DatabaseName = _connection.Database }
        );
    }

    /// <inheritdoc />
    public async Task Purge(CancellationToken token = default)
    {
        await ValidateMigration(token);
        await ExecuteSqlAsync(SqlStatement.DeleteAllTargets, token: token);
    }

    /// <inheritdoc />
    public async Task<IDbConnection> Connect(CancellationToken token = default)
    {
        return await OpenConnectionAsync(token);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Snapshot>> ListSnapshots(string? targetKey = null, CancellationToken token = default)
    {
        await ValidateMigration(token);
        await using var connection = await OpenConnectionAsync(token);
        var key = new { target_key = targetKey };
        return await connection.QueryAsync<Snapshot>(SqlStatement.ListSnapshots, key);
    }

    /// <inheritdoc />
    public async Task<Snapshot> GetSnapshotLatest(string targetKey, CancellationToken token = default)
    {
        await ValidateMigration(token);
        await using var connection = await OpenConnectionAsync(token);
        var key = new { target_key = targetKey };
        return await connection.QuerySingleAsync<Snapshot>(SqlStatement.GetLatestSnapshot, key);
    }

    /// <inheritdoc />
    public async Task<Snapshot> GetSnapshotById(int snapshotId, CancellationToken token = default)
    {
        await ValidateMigration(token);
        await using var connection = await OpenConnectionAsync(token);
        var key = new { snapshot_id = snapshotId };
        return await connection.QuerySingleAsync<Snapshot>(SqlStatement.GetSnapshotById, key);
    }

    public async Task AddSnapshot(Snapshot snapshot, ImportOption option = ImportOption.Append,
        CancellationToken token = default)
    {
        await ValidateMigration(token);
        await HandleSnapshotAction(snapshot.TargetKey, option, token);

        await using var connection = await OpenConnectionAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        try
        {
            // 1. Ensure Snapshot and Target records exist first (sets snapshot.SnapshotId)
            await ImportSnapshotAsync(snapshot, connection, transaction);

            // 2. Compile component data into DataTables
            var options = await GetTableOptions(connection, transaction);
            var tables = snapshot.Compile(options);

            // 3. Write component data using the Bulk Writer
            var writer = new SqlServerDbWriter(connection, (SqlTransaction)transaction);
            await writer.WriteAsync(tables, token);

            await transaction.CommitAsync(token);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteSnapshotsFor(string targetKey, CancellationToken token = default)
    {
        await ValidateMigration(token);
        var param = new { target_key = targetKey };
        await ExecuteSqlAsync(SqlStatement.DeleteTargetById, param, token);
    }

    /// <inheritdoc />
    public async Task DeleteSnapshotsBefore(DateTime importDate, string? targetKey = null,
        CancellationToken token = default)
    {
        await ValidateMigration(token);
        var param = new { target_key = targetKey, import_date = importDate };
        await ExecuteSqlAsync(SqlStatement.DeleteSnapshotsBefore, param, token);
    }

    /// <inheritdoc />
    public async Task DeleteSnapshotLatest(string targetKey, CancellationToken token = default)
    {
        await ValidateMigration(token);
        var param = new { target_key = targetKey };
        await ExecuteSqlAsync(SqlStatement.DeleteSnapshotByLatest, param, token);
    }

    /// <inheritdoc />
    public async Task DeleteSnapshot(int snapshotId, CancellationToken token = default)
    {
        await ValidateMigration(token);
        var param = new { snapshot_id = snapshotId };
        await ExecuteSqlAsync(SqlStatement.DeleteSnapshotById, param, token);
    }

    /// <summary>
    /// Imports a snapshot into the database, ensuring all relevant data is recorded and associated with the specified target.
    /// </summary>
    /// <param name="snapshot">The snapshot object containing the data to be imported.</param>
    /// <param name="conn">The database connection instance used for interaction with the database.</param>
    /// <param name="tran">The database transaction under which the operations will be executed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task ImportSnapshotAsync(Snapshot snapshot, SqlConnection conn, DbTransaction tran)
    {
        // Ensure the target entry exists and get the corresponding target id to use for the snapshot insert.
        var key = new { target_key = snapshot.TargetKey };
        await conn.ExecuteAsync(SqlStatement.EnsureTargetExists, key, tran);
        var targetId = await conn.QuerySingleAsync<int>(SqlStatement.GetTargetId, key, tran);

        // Post the provided snapshot to the database. Update the snapshot instance with the inserted ID.
        snapshot.SnapshotId = await conn.ExecuteScalarAsync<int>(SqlStatement.InsertSnapshot, new
        {
            target_id = targetId,
            target_type = snapshot.TargetType,
            target_name = snapshot.TargetName,
            is_partial = snapshot.IsPartial,
            schema_revision = snapshot.SchemaRevision,
            software_revision = snapshot.SoftwareRevision,
            export_date = snapshot.ExportDate,
            export_options = snapshot.ExportOptions,
            import_date = snapshot.ImportDate,
            import_user = snapshot.ImportUser,
            import_machine = snapshot.ImportMachine,
            source_hash = snapshot.SourceHash,
            source_data = snapshot.SourceData
        }, tran);

        // Post the snapshot metadata to the database.
        await conn.ExecuteAsync(SqlStatement.InsertSnapshotMetadata,
            snapshot.Metadata.Select(p => new
            {
                snapshot_id = snapshot.SnapshotId,
                property_name = p.Key,
                property_value = p.Value
            }).ToList(),
            tran);
    }

    /// <summary>
    /// Retrieves table options for the database, filtering to include only specific tables relevant to the system.
    /// </summary>
    /// <param name="connection">An open SQLite database connection used to query table names.</param>
    /// <param name="transaction">The database transaction context within which the query runs.</param>
    /// <returns>A <see cref="TableOptions"/> object specifying the tables to include for further operations.</returns>
    /// <exception cref="NotImplementedException">Thrown if the method is not yet fully implemented.</exception>
    private static async Task<TableOptions> GetTableOptions(SqlConnection connection, DbTransaction transaction)
    {
        var names = await connection.QueryAsync<string>(SqlStatement.GetTableNames, transaction);
        //todo we need to filter this to just known tables of our system and not predefined or other tables users may create.
        var include = names.ToArray();
        return new TableOptions { Include = include };
    }

    /// <summary>
    /// Handles snapshot actions based on the specified action type for a given target key.
    /// </summary>
    /// <param name="targetKey">The key identifying the target for the snapshot action.</param>
    /// <param name="action">The type of action to perform on the snapshot (Append, ReplaceLatest, or ReplaceAll).</param>
    /// <param name="token">A cancellation token that can be used to signal the operation should be canceled.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified action is not recognized.</exception>
    private async Task HandleSnapshotAction(string targetKey, ImportOption action, CancellationToken token)
    {
        switch (action)
        {
            case ImportOption.ReplaceLatest:
                await ExecuteSqlAsync(SqlStatement.DeleteSnapshotByLatest, new { target_key = targetKey }, token);
                break;
            case ImportOption.ReplaceAll:
                await ExecuteSqlAsync(SqlStatement.DeleteTargetById, new { target_key = targetKey }, token);
                break;
            case ImportOption.Append:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }

    /// <summary>
    /// Executes an SQL statement asynchronously within a transactional context.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="param">The parameters to bind to the SQL query.</param>
    /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task ExecuteSqlAsync(string sql, object? param = null, CancellationToken token = default)
    {
        await using var connection = await OpenConnectionAsync(token);
        await using var transaction = await connection.BeginTransactionAsync(token);

        try
        {
            await connection.ExecuteAsync(sql, param, transaction);
            await transaction.CommitAsync(token);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(token);
            throw;
        }
    }

    /// <summary>
    /// Validates connection to the database and then ensures that all pending migrations
    /// have been applied to the database.
    /// </summary>
    private async Task ValidateMigration(CancellationToken token = default)
    {
        try
        {
            await OpenConnectionAsync(token);
        }
        catch (SqlException e)
        {
            throw new InvalidOperationException(
                $"Failed to connect to database with error '{e.Message}' Ensure migration by running the 'migrate' command.");
        }

        await using var provider = BuildMigrationProvider(_connection.ToConnectionString());
        var runner = provider.GetRequiredService<IMigrationRunner>();

        if (runner.HasMigrationsToApplyUp())
        {
            throw new MigrationRequiredException(_connection.Source);
        }
    }

    /// <summary>
    /// Ensures that the associated SQL Server database is created. If the database does not exist, it creates it.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the database creation operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task EnsureCreated(CancellationToken token)
    {
        await using var connection = await OpenMasterConnectionAsync(token);

        await connection.ExecuteAsync(
            $"""
             IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName)
             BEGIN
                 CREATE DATABASE [{_connection.Database}]
             END
             """,
            new { DatabaseName = _connection.Database }
        );
    }

    /// <summary>
    /// Establishes and opens a connection to the master database of the specified SQL Server instance.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>Returns a <see cref="SqlConnection"/> object representing the opened connection to the master database.</returns>
    private async Task<SqlConnection> OpenMasterConnectionAsync(CancellationToken token)
    {
        var connectionString = _connection.ToConnectionString("master");
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(token);
        return connection;
    }

    /// <summary>
    /// Opens and returns a new SQL connection using the configured connection string.
    /// </summary>
    /// <param name="token">The cancellation token to observe while waiting for the connection to open.</param>
    /// <returns>An open <see cref="SqlConnection"/> instance associated with the configured connection string.</returns>
    private async Task<SqlConnection> OpenConnectionAsync(CancellationToken token)
    {
        var connection = new SqlConnection(_connection.ToConnectionString());
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
    private static ServiceProvider BuildMigrationProvider(string connectionString, TableOptions? options = null)
    {
        var services = new ServiceCollection();

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .WithVersionTable(new MigrationTableMetaData())
                .ScanIn(
                    typeof(MigrationTableMetaData).Assembly,
                    typeof(SqlServerDb).Assembly)
                .For.Migrations()
            );

        var tags = MigrationTag.GetTags(options);
        services.Configure<RunnerOptions>(opt => opt.Tags = tags.ToArray());
        return services.BuildServiceProvider();
    }
}