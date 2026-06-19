namespace LogixDb.Data.Abstractions;

/// <summary>
/// Defines the interface for managing database operations in the LogixDb framework.
/// Provides methods for tasks such as applying migrations, dropping databases,
/// and establishing connections.
/// </summary>
public interface IDbManager
{
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
    /// Adds a Target to the database by performing both a post and a restore operation.
    /// This is a convenience method for importing and immediately instantiating into relational tables.
    /// </summary>
    /// <param name="target">The Target to be added to the database.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ImportTarget(Target target, CancellationToken token = default);

    /// <summary>
    /// Permanently removes the target and its entire Target history, including all archive records
    /// and expanded instances.
    /// </summary>
    /// <param name="targetKey">The target key identifying the target and its Targets to purge.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteTarget(string targetKey, CancellationToken token = default);

    /// <summary>
    /// Removes all expanded Target instances for the specified target, effectively "contracting" all versions
    /// back to their archived state.
    /// </summary>
    /// <param name="targetKey">The target key identifying the Target instances to prune.</param>
    /// <param name="versionNumber"></param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteVersion(string targetKey, int versionNumber, CancellationToken token = default);

    /// <summary>
    /// Permanently removes all versions of the specified Target in the database that were created before the given version.
    /// </summary>
    /// <param name="targetKey">The unique identifier of the Target to truncate (format: targettype://targetname).</param>
    /// <param name="beforeVersion">The version threshold. All versions before this value will be removed.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteVersions(string targetKey, int beforeVersion, CancellationToken token = default);

    /// <summary>
    /// Truncates all historical data for the specified Target in the database prior to the given date.
    /// </summary>
    /// <param name="targetKey">The unique identifier of the target whose historical data will be truncated.</param>
    /// <param name="beforeDate">The cutoff date; historical data before this date will be removed.</param>
    /// <param name="token">A cancellation token to cancel the operation if necessary.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteVersions(string targetKey, DateTime beforeDate, CancellationToken token = default);

    /// <summary>
    /// Persists the provided import object into the database asynchronously.
    /// </summary>
    /// <param name="import">The instance of the <see cref="Import"/> object to be saved.</param>
    /// <param name="token">A cancellation token to signal task cancellation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// Inserts this import data if not already in the database. If found, it will only update the import
    /// status to reflect the current status of the provided import.
    /// </remarks>
    Task PutImport(Import import, CancellationToken token = default);

    /// <summary>
    /// Logs information about an import operation to the database.
    /// </summary>
    /// <param name="log">The details of the import operation, including the import ID, log level, and message.</param>
    /// <param name="token">A cancellation token to cancel the logging operation.</param>
    /// <returns>A task representing the asynchronous logging operation.</returns>
    Task LogImport(ImportLog log, CancellationToken token = default);
}