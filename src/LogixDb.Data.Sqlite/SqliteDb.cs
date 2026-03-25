using System.Data;
using System.Data.Common;
using Dapper;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Exceptions;
using LogixDb.Migrations;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace LogixDb.Data.Sqlite;

/// <summary>
/// Represents an SQLite-backed implementation of the <see cref="ILogixDb"/> interface.
/// This class provides methods to manage database migrations, snapshots, and data import/export processes
/// within an SQLite database.
/// </summary>
public sealed class SqliteDb(SqlConnectionInfo connection) : ILogixDb
{
    /// <summary>
    /// Represents the connection information required for interacting with an SQLite database.
    /// This variable contains details such as the database file path, credentials, and other
    /// configuration parameters encapsulated in a <see cref="SqlConnectionInfo"/> instance.
    /// It serves as the primary connection descriptor for database operations.
    /// </summary>
    private readonly SqlConnectionInfo _connection = connection ?? throw new ArgumentNullException(nameof(connection));

    /// <inheritdoc />
    public async Task Migrate(TableOptions? options = null, CancellationToken token = default)
    {
        await using var provider = BuildMigrationProvider(_connection.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        await ConfigurePersistentPerformancePragmas(token);
    }

    /// <inheritdoc />
    public async Task Migrate(long version, TableOptions? options = null, CancellationToken token = default)
    {
        await using var provider = BuildMigrationProvider(_connection.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        await ConfigurePersistentPerformancePragmas(token);
    }

    /// <inheritdoc />
    public Task Drop(CancellationToken token = default)
    {
        if (File.Exists(_connection.Source))
            File.Delete(_connection.Source);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task Purge(CancellationToken token = default)
    {
        await EnsureCreatedAndMigrated();
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
        await EnsureCreatedAndMigrated();
        await using var connection = await OpenConnectionAsync(token);
        var key = new { target_key = targetKey };
        return await connection.QueryAsync<Snapshot>(SqlStatement.ListSnapshots, key);
    }

    /// <inheritdoc />
    public async Task<Snapshot> GetSnapshotLatest(string targetKey, CancellationToken token = default)
    {
        await EnsureCreatedAndMigrated();
        await using var connection = await OpenConnectionAsync(token);
        var key = new { target_key = targetKey };
        return await connection.QuerySingleAsync<Snapshot>(SqlStatement.GetLatestSnapshot, key);
    }

    /// <inheritdoc />
    public async Task<Snapshot> GetSnapshotById(int snapshotId, CancellationToken token = default)
    {
        await EnsureCreatedAndMigrated();
        await using var connection = await OpenConnectionAsync(token);
        var key = new { snapshot_id = snapshotId };
        return await connection.QuerySingleAsync<Snapshot>(SqlStatement.GetSnapshotById, key);
    }

    /// <inheritdoc />
    public async Task AddSnapshot(Snapshot snapshot, ImportOption option = ImportOption.Append,
        CancellationToken token = default)
    {
        await EnsureCreatedAndMigrated();
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
            var writer = new SqliteDbWriter(connection, (SqliteTransaction)transaction);
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
        await EnsureCreatedAndMigrated();
        var param = new { target_key = targetKey };
        await ExecuteSqlAsync(SqlStatement.DeleteTargetById, param, token);
    }

    /// <inheritdoc />
    public async Task DeleteSnapshotsBefore(DateTime importDate, string? targetKey = null,
        CancellationToken token = default)
    {
        await EnsureCreatedAndMigrated();
        var param = new { target_key = targetKey, import_date = importDate };
        await ExecuteSqlAsync(SqlStatement.DeleteSnapshotsBefore, param, token);
    }

    /// <inheritdoc />
    public async Task DeleteSnapshotLatest(string targetKey, CancellationToken token = default)
    {
        await EnsureCreatedAndMigrated();
        var param = new { target_key = targetKey };
        await ExecuteSqlAsync(SqlStatement.DeleteSnapshotByLatest, param, token);
    }

    /// <inheritdoc />
    public async Task DeleteSnapshot(int snapshotId, CancellationToken token = default)
    {
        await EnsureCreatedAndMigrated();
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
    private static async Task ImportSnapshotAsync(Snapshot snapshot, SqliteConnection conn, DbTransaction tran)
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
    private static async Task<TableOptions> GetTableOptions(SqliteConnection connection, DbTransaction transaction)
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
    /// Establishes and opens an asynchronous connection to the SQLite database using the configured connection string.
    /// </summary>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an open database connection.</returns>
    private async Task<SqliteConnection> OpenConnectionAsync(CancellationToken token)
    {
        var connection = new SqliteConnection(_connection.ToConnectionString());
        await connection.OpenAsync(token);
        return connection;
    }

    /// <summary>
    /// Ensures that all pending migrations have been applied to the database.
    /// </summary>
    /// <exception cref="MigrationRequiredException">
    /// Thrown when there are unapplied migrations that need to be applied to bring
    /// the database to the required state.
    /// </exception>
    private async Task EnsureCreatedAndMigrated()
    {
        if (!File.Exists(_connection.Source))
            throw new FileNotFoundException($"Database file not found: {_connection.Source}");

        await using var provider = BuildMigrationProvider(_connection.ToConnectionString());
        var runner = provider.GetRequiredService<IMigrationRunner>();

        if (runner.HasMigrationsToApplyUp())
        {
            throw new MigrationRequiredException(_connection.Source);
        }
    }

    /// <summary>
    /// Creates a migration provider for managing database migrations using FluentMigrator.
    /// Configures the service collection with the necessary settings for SQLite integration,
    /// including connection string, version table metadata, and migration scanning.
    /// </summary>
    /// <param name="connectionString">The connection string for the SQLite database.</param>
    /// <param name="options"></param>
    /// <returns>A configured <see cref="ServiceProvider"/> instance to execute migrations.</returns>
    private static ServiceProvider BuildMigrationProvider(string connectionString, TableOptions? options = null)
    {
        var services = new ServiceCollection();

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(connectionString)
                .WithVersionTable(new MigrationTableMetaData())
                .ScanIn(
                    typeof(MigrationTableMetaData).Assembly,
                    typeof(SqliteDb).Assembly)
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
    private async Task ConfigurePersistentPerformancePragmas(CancellationToken token)
    {
        await using var connection = await OpenConnectionAsync(token);

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