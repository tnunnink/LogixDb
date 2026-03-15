using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Imports instruction argument data from L5X snapshots into SQL Server database.
/// </summary>
internal class SqlServerArgumentImport : SqlServerImport
{
    private readonly ArgumentMap _map = new();

    private static readonly RungMap RungMap = new();
    private static readonly InstructionMap InstructionMap = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var id = snapshot.SnapshotId;

        var rungs = source.Query<Rung>().Select(r => new RungRecord(id, r));

        var records = rungs.SelectMany(rung =>
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
        });

        return ImportRecords(records, _map, session, token);
    }
}