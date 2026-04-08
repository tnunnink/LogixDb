using System.Data;

namespace LogixDb.Data.Abstractions;

/// <summary>
/// Defines a contract for transforming a <see cref="Snapshot"/> into a collection of <see cref="DataTable"/> objects
/// that can be written to a database. Implementations of this interface are responsible for mapping the snapshot data
/// into the appropriate table structure for database persistence.
/// </summary>
internal interface ISnapshotTransformer
{
    /// <summary>
    /// Transforms the specified <see cref="Snapshot"/> into a collection of <see cref="DataTable"/> objects
    /// representing the data in a format suitable for database import operations.
    /// </summary>
    /// <param name="snapshot">The snapshot containing the data to be transformed into data tables.</param>
    /// <returns>A collection of <see cref="DataTable"/> objects representing the transformed snapshot data.</returns>
    IEnumerable<DataTable> Transform(Snapshot snapshot);
}