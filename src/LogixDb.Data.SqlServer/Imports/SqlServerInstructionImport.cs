using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;


/// <summary>
/// Represents a class for importing instruction data into a SqlServer database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import instructions from rungs into a SqlServer database
/// by using a specific set of preconfigured SQL commands and mappings. It extracts all instructions from
/// each rung in the snapshot, flattens them into individual instruction records, and associates them with
/// their parent rung hash for relational integrity.
/// </remarks>
internal class SqlServerInstructionImport() : SqlServerImport<InstructionRecord>(new InstructionMap())
{
    private static readonly RungMap RungMap = new();

    /// <inheritdoc />
    protected override DataTable GetData(Snapshot snapshot)
    {
        var source = snapshot.GetSource();

        var rungs = source.Query<Rung>()
            .Select(r => new RungRecord(snapshot.SnapshotId, r))
            .ToList();

        var flattened = rungs.SelectMany(rung =>
        {
            var rungHash = RungMap.ComputeHash(rung);
            return rung.Rung.Instructions().Select(i => new InstructionRecord(snapshot.SnapshotId, rungHash, i));
        }).ToList();

        return Map.GenerateTable(flattened);
    }
}