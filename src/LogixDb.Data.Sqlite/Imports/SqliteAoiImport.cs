using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Represents a class for importing Add-On Instruction (AOI) data into a SQLite database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import AOIs into a SQLite database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
internal class SqliteAoiImport() : SqliteImport<AoiRecord>(new AoiMap())
{
    /// <inheritdoc />
    protected override DataTable GetData(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        
        var records = source.Query<AddOnInstruction>()
            .Select(a => new AoiRecord(snapshot.SnapshotId, a))
            .ToList();

        return Map.GenerateTable(records);
    }
}