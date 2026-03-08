using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a specialized implementation of <see cref="SqlServerImport{TElement}"/>
/// for importing <see cref="Tag"/> elements from an L5X file into a SQL Server database.
/// </summary>
/// <remarks>
/// This class uses the <see cref="TagMap"/> to define the database mapping for <see cref="Tag"/> elements
/// and processes the import by extracting tag data and its associated members from the L5X content.
/// </remarks>
/// <seealso cref="SqlServerImport{TElement}"/>
internal class SqlServerTagImport() : SqlServerImport<TagRecord>(new TagMap())
{
    /// <inheritdoc />
    protected override DataTable GetData(Snapshot snapshot)
    {
        var source = snapshot.GetSource();

        var records = source.Query<Tag>()
            .SelectMany(t => t.Members())
            .Select(t => new TagRecord(snapshot.SnapshotId, t))
            .ToList();

        return Map.GenerateTable(records);
    }
}