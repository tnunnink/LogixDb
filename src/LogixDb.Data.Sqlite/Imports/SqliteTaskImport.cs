using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of task records from a LogixDb snapshot into an SQLite database.
/// This class processes task entities by querying them from the snapshot source and inserting
/// them into the database using the configured task table mapping.
/// </summary>
internal class SqliteTaskImport : SqliteImport
{
    private readonly TaskMap _taskMap = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_taskMap, session);
        var source = snapshot.GetSource();
        var records = source.Query<L5Sharp.Core.Task>().Select(t => new TaskRecord(snapshot.SnapshotId, t)).ToList();
        await ImportRecords(records, _taskMap, command, token);
    }
}