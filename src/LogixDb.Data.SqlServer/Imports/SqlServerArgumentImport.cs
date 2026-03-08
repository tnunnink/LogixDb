using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Imports instruction argument data from L5X snapshots into SQL Server database.
/// </summary>
/// <remarks>
/// This importer extracts all instruction arguments from rungs within a snapshot,
/// flattens the hierarchical structure (Rung -> Instruction -> Argument), and
/// generates a data table for bulk import. Each argument is linked to its parent
/// instruction via computed hash values and includes its position index.
/// </remarks>
internal class SqlServerArgumentImport() : SqlServerImport<ArgumentRecord>(new ArgumentMap())
{
    private static readonly RungMap RungMap = new();
    private static readonly InstructionMap InstructionMap = new();

    /// <inheritdoc />
    protected override DataTable GetData(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var id = snapshot.SnapshotId;

        var rungs = source.Query<Rung>().Select(r => new RungRecord(id, r)).ToList();

        var flattened = rungs.SelectMany(rung =>
        {
            var rh = RungMap.ComputeHash(rung);
            return rung.Rung.Instructions().SelectMany(x =>
            {
                var ih = InstructionMap.ComputeHash(new InstructionRecord(snapshot.SnapshotId, rh, x));
                return x.Arguments.Select((a, i) => new ArgumentRecord(snapshot.SnapshotId, ih, (byte)i, a));
            });
        }).ToList();

        return Map.GenerateTable(flattened);
    }
}