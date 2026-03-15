using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of instruction argument records from a LogixDb snapshot into an SQLite database.
/// This class processes arguments by querying rungs and instructions from the snapshot source and inserting
/// them into the database using the configured argument table mapping.
/// </summary>
internal class SqliteArgumentImport : SqliteImport
{
    private static readonly RungMap RungMap = new();
    private static readonly InstructionMap InstructionMap = new();
    private readonly ArgumentMap _map = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        await using var command = BuildCommand(_map, session);
        var source = snapshot.GetSource();
        var id = snapshot.SnapshotId;

        var records = source.Query<Rung>()
            .Select(r => new RungRecord(id, r))
            .SelectMany(rung =>
            {
                var rungHash = RungMap.ComputeHash(rung);

                return rung.Rung.Instructions().SelectMany((instruction, index) =>
                {
                    var instructionHash = InstructionMap.ComputeHash(
                        new InstructionRecord(snapshot.SnapshotId, rungHash, (short)index, instruction)
                    );

                    return instruction.Arguments.Select((a, i) =>
                        new ArgumentRecord(snapshot.SnapshotId, instructionHash, (byte)i, a)
                    );
                });
            }).ToList();

        await ImportRecords(records, _map, command, token);
    }
}