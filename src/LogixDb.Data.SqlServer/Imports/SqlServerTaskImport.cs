using L5Sharp.Core;
using LogixDb.Data.Maps;
using Task = L5Sharp.Core.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing task data into a SqlServer database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import tasks into a SqlServer database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
internal class SqlServerTaskImport() : SqlServerImport<TaskRecord>(new TaskMap())
{
    /// <inheritdoc />
    protected override IEnumerable<TaskRecord> GetRecords(Snapshot snapshot)
    {
        return snapshot.GetSource().Query<Task>()
            .Select(x => new TaskRecord(snapshot.SnapshotId, x))
            .ToList();
    }
}
