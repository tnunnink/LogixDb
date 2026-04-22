using System.Data;

namespace LogixDb.Data.Abstractions;

/// <summary>
/// Defines a contract for writing a collection of <see cref="DataTable"/> objects to a database.
/// Implementations of this interface are responsible for persisting data tables to the underlying
/// database system using appropriate bulk insert or write operations.
/// </summary>
public interface IDbWriter
{
    /// <summary>
    /// Writes the specified collection of <see cref="DataTable"/> objects to the database asynchronously.
    /// </summary>
    /// <param name="tables">The collection of <see cref="DataTable"/> objects to be written to the database.</param>
    /// <param name="token">A <see cref="CancellationToken"/> that can be used to observe cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous write operation.</returns>
    Task WriteAsync(IEnumerable<DataTable> tables, CancellationToken token);
}