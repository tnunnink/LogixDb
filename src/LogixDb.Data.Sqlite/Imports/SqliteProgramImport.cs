using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Represents a class for importing program data into a SQLite database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import programs into a SQLite database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
internal class SqliteProgramImport() : SqliteImport<ProgramRecord>(new ProgramMap())
{
    /// <inheritdoc />
    protected override DataTable GetData(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        
        var records = source.Query<Program>()
            .Select(p => new ProgramRecord(snapshot.SnapshotId, p))
            .ToList();

        return Map.GenerateTable(records);
    }
}
