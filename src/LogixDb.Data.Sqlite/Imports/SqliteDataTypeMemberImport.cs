using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Represents a class for importing data type member data into a SQLite database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import data type members into a SQLite database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
internal class SqliteDataTypeMemberImport() : SqliteImport<DataTypeMemberRecord>(new DataTypeMemberMap())
{
    /// <inheritdoc />
    protected override DataTable GetData(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        
        var records = source.Query<DataType>()
            .SelectMany(dt => dt.Members)
            .Select(m => new DataTypeMemberRecord(snapshot.SnapshotId, m))
            .ToList();

        return Map.GenerateTable(records);
    }
}