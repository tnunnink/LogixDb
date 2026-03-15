using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of instruction records from a LogixDb snapshot into an SQLite database.
/// This class processes instruction entities by querying rungs from the snapshot source and inserting
/// them into the database using the configured instruction table mapping.
/// </summary>
internal class SqliteInstructionImport : SqliteImport
{
    private static readonly RungMap RungMap = new();
    private readonly InstructionMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_map, session);
        var source = snapshot.GetSource();
        
        var records = source.Query<Rung>()
            .Select(r => new RungRecord(snapshot.SnapshotId, r))
            .SelectMany(rung =>
            {
                var rungHash = RungMap.ComputeHash(rung);

                return rung.Rung.Instructions().Select((i, x) =>
                    new InstructionRecord(snapshot.SnapshotId, rungHash, (short)x, i)
                );
            }).ToList();

        await ImportRecords(records, _map, command, token);
    }
}