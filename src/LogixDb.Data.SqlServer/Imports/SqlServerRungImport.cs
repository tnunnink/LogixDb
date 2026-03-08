using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing rung data into a SqlServer database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import rungs into a SqlServer database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
internal class SqlServerRungImport() : SqlServerImport<RungRecord>(new RungMap())
{
    /// <inheritdoc />
    protected override DataTable GetData(Snapshot snapshot)
    {
        var source = snapshot.GetSource();

        var records = source.Query<Rung>()
            .Select(x => new RungRecord(snapshot.SnapshotId, x))
            .ToList();

        return Map.GenerateTable(records);
    }
}