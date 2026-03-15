using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Represents a class for importing instruction data into a SqlServer database.
/// </summary>
internal class SqlServerInstructionImport : SqlServerImport
{
    private readonly InstructionMap _map = new();

    private static readonly RungMap RungMap = new();

    /// <inheritdoc />
    public override Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var rungs = source.Query<Rung>().Select(r => new RungRecord(snapshot.SnapshotId, r));

        var records = rungs.SelectMany(rung =>
        {
            var rungHash = RungMap.ComputeHash(rung);

            return rung.Rung.Instructions().Select((i, x) =>
                new InstructionRecord(snapshot.SnapshotId, rungHash, (short)x, i)
            );
        });

        return ImportRecords(records, _map, session, token);
    }
}