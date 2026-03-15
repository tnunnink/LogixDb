using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Handles the import of Add-On Instruction (AOI) logic data into a SQL Server database.
/// Extends the <see cref="SqlServerImport"/> base class to provide specific functionality
/// for importing AOI logic records within the LogixDb context.
/// </summary>
internal class SqlServerAoiRungImport : SqlServerImport
{
    private readonly AoiRungMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();

        var records = source.Query<AddOnInstruction>()
            .SelectMany(a => a.Routines.Where(r => r.Type == RoutineType.RLL)
                .SelectMany(r => r.Rungs
                    .Select(rung => new AoiRungRecord(snapshot.SnapshotId, a.Name, r.Name, rung))));

        await ImportRecords(records, _map, session, token);
    }
}