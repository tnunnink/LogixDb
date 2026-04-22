using System.Data;

namespace LogixDb.Data.Abstractions;

/// <summary>
/// Defines a contract for transforming a <see cref="Target"/> into a collection of <see cref="DataTable"/> objects
/// that can be written to a database. Implementations of this interface are responsible for mapping the Target data
/// into the appropriate table structure for database persistence.
/// </summary>
internal interface IDbTransformer
{
    /// <summary>
    /// Transforms the specified <see cref="Target"/> into a collection of <see cref="DataTable"/> objects
    /// representing the data in a format suitable for database import operations.
    /// </summary>
    /// <param name="target">The Target containing the data to be transformed into data tables.</param>
    /// <returns>A collection of <see cref="DataTable"/> objects representing the transformed Target data.</returns>
    IEnumerable<DataTable> Transform(Target target);
}
