using System.Data;

namespace LogixDb.Data.Abstractions;

/// <summary>
/// Represents the main interface for interacting with a LogixDb database instance.
/// Provides methods for database lifecycle management, migrations, and snapshot operations.
/// </summary>
public interface ILogixDb
{
    /// <summary>
    /// Applies pending database migrations to update the schema to the latest version.
    /// </summary>
    /// <param name="options">Configuration options that specify which tables should be created during migration.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Migrate(ComponentOptions options = ComponentOptions.All, CancellationToken token = default);

    /// <summary>
    /// Applies pending database migrations up to the specified schema version.
    /// </summary>
    /// <param name="version">The target schema version to migrate to.</param>
    /// <param name="options">Configuration options that specify which tables should be created during migration.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Migrate(long version, ComponentOptions options = ComponentOptions.All, CancellationToken token = default);

    /// <summary>
    /// Drops or deletes the database, removing all tables and data.
    /// </summary>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Drop(CancellationToken token = default);

    /// <summary>
    /// Purges all data from the database while preserving the schema structure.
    /// </summary>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Purge(CancellationToken token = default);

    /// <summary>
    /// Establishes a connection to the database and returns an active database connection instance.
    /// </summary>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the opened database connection.</returns>
    Task<IDbConnection> Connect(CancellationToken token = default);

    /// <summary>
    /// Lists all snapshots in the database, optionally filtered by target key.
    /// </summary>
    /// <param name="targetKey">Optional target key to filter snapshots (format: targettype://targetname).</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation containing a collection of snapshots.</returns>
    Task<IEnumerable<Snapshot>> ListSnapshots(string? targetKey = null, CancellationToken token = default);

    /// <summary>
    /// Retrieves a snapshot from the database based on the specified target key and version.
    /// </summary>
    /// <param name="targetKey">The unique identifier of the target whose snapshot is being retrieved.</param>
    /// <param name="version">The version number of the snapshot to retrieve.</param>
    /// <param name="token">A cancellation token to cancel the operation, if needed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the requested snapshot.</returns>
    Task<Snapshot> GetSnapshot(string targetKey, int version = 0, CancellationToken token = default);

    /// <summary>
    /// Adds a snapshot to the database and archives previous snapshots for the same target key.
    /// This operation prunes the detailed content of the most recent snapshot while 
    /// preserving its snapshot record and L5X blob for historical reference.
    /// </summary>
    /// <param name="snapshot">The snapshot to be archived in the database.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddSnapshot(Snapshot snapshot, CancellationToken token = default);

    /// <summary>
    /// Deletes all snapshots matching the specified target key.
    /// </summary>
    /// <param name="targetKey">The target key identifying the snapshot(s) to delete (format: targettype://targetname).</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteSnapshots(string targetKey, CancellationToken token = default);

    /// <summary>
    /// Deletes a specific snapshot by its target key and version number.
    /// </summary>
    /// <param name="targetKey">The target key identifying the snapshot to delete (format: targettype://targetname).</param>
    /// <param name="version">The version number of the snapshot to delete.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteSnapshot(string targetKey, int version, CancellationToken token = default);

    /// <summary>
    /// Deletes snapshots created before the specified cutoff date. Optionally filters snapshots by a target key.
    /// </summary>
    /// <param name="importDate">The date before which snapshots will be deleted.</param>
    /// <param name="targetKey">An optional key to filter snapshots for a specific target. If null, all matching snapshots will be affected.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteSnapshotsBefore(DateTime importDate, string? targetKey = null, CancellationToken token = default);
}