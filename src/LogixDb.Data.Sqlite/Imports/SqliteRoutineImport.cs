using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of routine records from a LogixDb snapshot into an SQLite database.
/// This class processes routine entities by querying them from the snapshot source and inserting
/// them into the database using the configured routine table mapping.
/// </summary>
internal class SqliteRoutineImport : SqliteImport
{
    private readonly RoutineMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var records = source.Query<Routine>().Select(x => new RoutineRecord(snapshot.SnapshotId, x)).ToList();
        await ImportRecords(records, _map, session, token);
    }
}