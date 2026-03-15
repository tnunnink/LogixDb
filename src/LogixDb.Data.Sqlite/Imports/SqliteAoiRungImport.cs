using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

internal class SqliteAoiRungImport : SqliteImport
{
    private readonly AoiRungMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_map, session);
        var source = snapshot.GetSource();

        var records = source.Query<AddOnInstruction>()
            .SelectMany(a => a.Routines.Where(r => r.Type == RoutineType.RLL)
                .SelectMany(r => r.Rungs
                    .Select(rung => new AoiRungRecord(snapshot.SnapshotId, a.Name, r.Name, rung))));

        await ImportRecords(records, _map, command, token);
    }
}