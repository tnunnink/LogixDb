using System.Data;
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
public sealed class SqliteDb(DbConnectionInfo connection) : ILogixDb
{
    /// <summary>
    /// Represents the connection information required for interacting with an SQLite database.
    /// This variable contains details such as the database file path, credentials, and other
    /// configuration parameters encapsulated in a <see cref="DbConnectionInfo"/> instance.
    /// It serves as the primary connection descriptor for database operations.
    /// </summary>
    private readonly DbConnectionInfo _connection = connection ?? throw new ArgumentNullException(nameof(connection));

    /// <inheritdoc />
    public async Task Migrate(ComponentOptions options = ComponentOptions.All,
        CancellationToken token = default)
    {
        await using var provider = BuildMigrationProvider(_connection.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
        await ConfigureDatabase(token);
    }

    /// <inheritdoc />
    public async Task Migrate(long version, ComponentOptions options = ComponentOptions.All,
        CancellationToken token = default)
    {
        await using var provider = BuildMigrationProvider(_connection.ToConnectionString(), options);
        var runner = provider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp(version);
        await ConfigureDatabase(token);
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
        await EnsureDatabase();
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
        await EnsureDatabase();
        await using var connection = await OpenConnectionAsync(token);
        var key = new { target_key = targetKey };
        return await connection.QueryAsync<Snapshot>(SqlStatement.ListSnapshots, key);
    }

    /// <inheritdoc />
    public async Task<Snapshot> GetSnapshotLatest(string targetKey, CancellationToken token = default)
    {
        await EnsureDatabase();
        await using var connection = await OpenConnectionAsync(token);
        var key = new { target_key = targetKey };
        return await connection.QuerySingleAsync<Snapshot>(SqlStatement.GetLatestSnapshot, key);
    }

    /// <inheritdoc />
    public async Task<Snapshot> GetSnapshotById(int snapshotId, CancellationToken token = default)
    {
        await EnsureDatabase();
        await using var connection = await OpenConnectionAsync(token);
        var key = new { snapshot_id = snapshotId };
        return await connection.QuerySingleAsync<Snapshot>(SqlStatement.GetSnapshotById, key);
    }

    /// <inheritdoc />
    public async Task AddSnapshot(Snapshot snapshot, CancellationToken token = default)
    {
        await EnsureDatabase();
        await using var session = await SqliteDbSession.Start(_connection.ToConnectionString(), token);

        try
        {
            await PruneSnapshotsAsync(snapshot.TargetKey, session);
            await CreateSnapshotAsync(session, snapshot);
            await SaveSnapshotAsync(snapshot, session, token);
            await session.Commit(token);
        }
        catch (Exception)
        {
            await session.Rollback(token);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task DeleteSnapshotsFor(string targetKey, CancellationToken token = default)
    {
        await EnsureDatabase();
        var param = new { target_key = targetKey };
        await ExecuteSqlAsync(SqlStatement.DeleteTargetById, param, token);
    }

    /// <inheritdoc />
    public async Task DeleteSnapshotsBefore(DateTime importDate, string? targetKey = null,
        CancellationToken token = default)
    {
        await EnsureDatabase();
        var param = new { target_key = targetKey, import_date = importDate };
        await ExecuteSqlAsync(SqlStatement.DeleteSnapshotsBefore, param, token);
    }

    /// <inheritdoc />
    public async Task DeleteSnapshotLatest(string targetKey, CancellationToken token = default)
    {
        await EnsureDatabase();
        var param = new { target_key = targetKey };
        await ExecuteSqlAsync(SqlStatement.DeleteSnapshotByLatest, param, token);
    }

    /// <inheritdoc />
    public async Task DeleteSnapshot(int snapshotId, CancellationToken token = default)
    {
        await EnsureDatabase();
        var param = new { snapshot_id = snapshotId };
        await ExecuteSqlAsync(SqlStatement.DeleteSnapshotById, param, token);
    }

    /// <summary>
    /// Imports a snapshot into the database, ensuring all relevant data is recorded and associated with the specified target.
    /// </summary>
    /// <param name="session">The database session used to execute the import operations.</param>
    /// <param name="snapshot">The snapshot object containing the data to be imported.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static async Task CreateSnapshotAsync(SqliteDbSession session, Snapshot snapshot)
    {
        // Ensure the target entry exists and get the corresponding target id to use for the snapshot insert.
        await session.ExecuteAsync(SqlStatement.EnsureTargetExists,
            new { target_id = Guid.NewGuid(), target_key = snapshot.TargetKey }
        );

        // Retrieve the target key back from the database since it could already exist.
        var targetId = Guid.Parse(
            await session.GetAsync<string>(SqlStatement.GetTargetId, new { target_key = snapshot.TargetKey })
        );

        // Post the provided snapshot to the database. Update the snapshot instance with the inserted ID.
        snapshot.SnapshotId = await session.Connection.ExecuteScalarAsync<int>(SqlStatement.InsertSnapshot, new
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
        }, session.Transaction);

        // Post the snapshot metadata to the database.
        await session.ExecuteAsync(SqlStatement.InsertSnapshotMetadata,
            snapshot.Metadata.Select(p => new
            {
                property_id = Guid.NewGuid(),
                snapshot_id = snapshot.SnapshotId,
                property_name = p.Key,
                property_value = p.Value
            }).ToList());
    }

    /// <summary>
    /// Saves snapshot data into the database using the provided session.
    /// </summary>
    /// <param name="snapshot">The snapshot containing the data to be saved.</param>
    /// <param name="session">The database session used for executing the saving operation.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private static async Task SaveSnapshotAsync(Snapshot snapshot, SqliteDbSession session, CancellationToken token)
    {
        var writer = new SqliteDbWriter(session);
        var tableNames = await GetTableNames(session);
        var tables = snapshot.Compile(tableNames);
        await writer.WriteAsync(tables, token);
    }

    /// <summary>
    /// Removes all existing snapshots associated with the specified target key from the database.
    /// </summary>
    /// <param name="targetKey">The key identifying the target for which snapshots should be pruned.</param>
    /// <param name="session">The database session used to execute the pruning operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private static async Task PruneSnapshotsAsync(string targetKey, SqliteDbSession session)
    {
        var key = new { target_key = targetKey };
        var snapshots = await session.GetAllAsync<Snapshot>(SqlStatement.ListSnapshots, key);
        var ids = snapshots.Select(s => s.SnapshotId).ToList();

        List<string> targets = ["controller", "data_type", "aoi", "module", "tag", "program", "task", "operand"];
        var tables = await GetTableNames(session);

        // filter and order the tables to prevent FK reference issues (mainly with task and program)
        var orderedTables = targets
            .Where(target => tables.Contains(target, StringComparer.OrdinalIgnoreCase))
            .ToList();

        foreach (var table in orderedTables)
        {
            if (!targets.Contains(table)) continue;
            var sql = $"DELETE FROM {table} WHERE snapshot_id IN @ids";
            await session.ExecuteAsync(sql, new { ids });
        }
    }

    /// <summary>
    /// Retrieves the names of all relevant tables from the SQLite database.
    /// </summary>
    /// <param name="session">The SQLite database session to execute the query against.</param>
    /// <returns>A collection of table names as strings.</returns>
    private static async Task<ICollection<string>> GetTableNames(SqliteDbSession session)
    {
        var names = await session.GetAllAsync<string>(SqlStatement.GetTableNames);
        return names.ToArray();
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
        await using var session = await SqliteDbSession.Start(_connection.ToConnectionString(), token);

        try
        {
            await session.ExecuteAsync(sql, param);
            await session.Commit(token);
        }
        catch (Exception)
        {
            await session.Rollback(token);
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
    private async Task EnsureDatabase()
    {
        if (!File.Exists(_connection.Source))
            throw new FileNotFoundException($"Database file not found: {_connection.Source}");

        // Just make sure the required tables exist. The component tables could be filtered out, which is fine.
        await using var provider = BuildMigrationProvider(_connection.ToConnectionString(), ComponentOptions.None);
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
    private async Task ConfigureDatabase(CancellationToken token)
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