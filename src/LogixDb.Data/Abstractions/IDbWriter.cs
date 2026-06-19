namespace LogixDb.Data.Abstractions;

/// <summary>
/// Provides functionality for writing target data to a database.
/// Implementations of this interface are responsible for compiling the target data
/// into data tables and persisting them to the underlying data store within a transactional context.
/// </summary>
public interface IDbWriter
{
    /// <summary>
    /// Asynchronously writes the specified target data to the database.
    /// The target is compiled into data tables which are then written to the database
    /// using the appropriate merge operations to ensure data integrity.
    /// </summary>
    /// <param name="target">The target data to be written to the database.</param>
    /// <param name="token">The cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteAsync(Target target, CancellationToken token);
}