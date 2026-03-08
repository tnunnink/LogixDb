using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing program data into a SqlServer database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import programs into a SqlServer database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
internal class SqlServerProgramImport() : SqlServerImport<ProgramRecord>(new ProgramMap())
{
    /// <inheritdoc />
    protected override IEnumerable<ProgramRecord> GetRecords(Snapshot snapshot)
    {
        return snapshot.GetSource().Query<Program>()
            .Select(x => new ProgramRecord(snapshot.SnapshotId, x))
            .ToList();
    }
}
