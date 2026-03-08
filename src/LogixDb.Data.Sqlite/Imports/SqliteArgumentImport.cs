using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Represents an implementation of the <see cref="SqliteImport{TRecord}"/> class specialized for importing
/// argument data into a SQLite database.
/// </summary>
public class SqliteArgumentImport() : SqliteImport<ArgumentRecord>(new ArgumentMap())
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