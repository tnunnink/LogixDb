using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of rung records from a LogixDb snapshot into an SQLite database.
/// This class processes rung entities by querying them from the snapshot source and inserting
/// them into the database using the configured rung table mapping.
/// </summary>
internal class SqliteRungImport : SqliteImport
{
    private readonly RungMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_map, session);
        var source = snapshot.GetSource();
        
        var records = source.Programs
            .SelectMany(p => p.Routines.Where(r => r.Type == RoutineType.RLL))
            .SelectMany(r => r.Rungs)
            .Select(r => new RungRecord(snapshot.SnapshotId, r))
            .ToList();
        
        await ImportRecords(records, _map, command, token);
    }
}