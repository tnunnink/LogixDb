using System.Data;

namespace LogixDb.Data.Abstractions;

/// <summary>
/// Defines the interface for managing database operations in the LogixDb framework.
/// Provides methods for tasks such as applying migrations, dropping databases,
/// and establishing connections.
/// </summary>
public interface IDbManager
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
    /// Establishes a connection to the database and returns an active database connection instance.
    /// </summary>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the opened database connection.</returns>
    Task<IDbConnection> Connect(CancellationToken token = default);

    /// <summary>
    /// Lists all Targets in the database, optionally filtered by target key.
    /// </summary>
    /// <param name="targetKey">Optional target key to filter Targets (format: targettype://targetname).</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation containing a collection of Targets.</returns>
    Task<IEnumerable<Target>> ListTargets(string? targetKey = null, CancellationToken token = default);

    /// <summary>
    /// Retrieves a Target from the database based on the specified target key and version.
    /// </summary>
    /// <param name="targetKey">The unique identifier of the target whose Target is being retrieved.</param>
    /// <param name="version">The version number of the Target to retrieve.</param>
    /// <param name="token">A cancellation token to cancel the operation, if needed.</param>
    /// <returns>A task representing the asynchronous operation, with a result of the requested Target.</returns>
    Task<Target?> GetTarget(string targetKey, int version = 0, CancellationToken token = default);

    /// <summary>
    /// Posts a Target's metadata and L5X blob to the database without instantiating it into relational tables.
    /// This represents the "archive" state of a Target.
    /// </summary>
    /// <param name="target">The Target to be posted to the database.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PostTarget(Target target, CancellationToken token = default);

    /// <summary>
    /// Adds a Target to the database by performing both a post and a restore operation.
    /// This is a convenience method for importing and immediately instantiating into relational tables.
    /// </summary>
    /// <param name="target">The Target to be added to the database.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ImportTarget(Target target, CancellationToken token = default);

    /// <summary>
    /// Restores a Target's compressed blob into relational tables. This makes the Target data queryable
    /// through the component tables.
    /// </summary>
    /// <param name="targetKey">The target key of the Target to restore.</param>
    /// <param name="version">The version number of the Target to restore. If 0, the latest version is restored.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RestoreTarget(string targetKey, int version = 0, CancellationToken token = default);

    /// <summary>
    /// Archives a Target instance, removing its instantiated relational data while preserving the historical
    /// Target metadata and L5X blob. This triggers a cascade delete on all related component tables.
    /// </summary>
    /// <param name="targetKey">The target key of the Target to archive.</param>
    /// <param name="version">The version number of the Target to archive. If 0, the latest version is archived.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ArchiveTarget(string targetKey, int version = 0, CancellationToken token = default);

    /// <summary>
    /// Removes all expanded Target instances for the specified target, effectively "contracting" all versions
    /// back to their archived state.
    /// </summary>
    /// <param name="targetKey">The target key identifying the Target instances to prune.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PruneTarget(string targetKey, CancellationToken token = default);

    /// <summary>
    /// Permanently removes the target and its entire Target history, including all archive records
    /// and expanded instances.
    /// </summary>
    /// <param name="targetKey">The target key identifying the target and its Targets to purge.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteTarget(string targetKey, CancellationToken token = default);

    /// <summary>
    /// Permanently removes all versions of the specified Target in the database that were created before the given version.
    /// </summary>
    /// <param name="targetKey">The unique identifier of the Target to truncate (format: targettype://targetname).</param>
    /// <param name="beforeVersion">The version threshold. All versions before this value will be removed.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task TruncateTarget(string targetKey, int beforeVersion, CancellationToken token = default);

    /// <summary>
    /// Truncates all historical data for the specified Target in the database prior to the given date.
    /// </summary>
    /// <param name="targetKey">The unique identifier of the target whose historical data will be truncated.</param>
    /// <param name="beforeDate">The cutoff date; historical data before this date will be removed.</param>
    /// <param name="token">A cancellation token to cancel the operation if necessary.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task TruncateTarget(string targetKey, DateTime beforeDate, CancellationToken token = default);
}