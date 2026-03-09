using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the <see cref="ArgumentRecord"/> entity,
/// defining the structure and behavior for database interactions with the "argument" table.
/// </summary>
/// <remarks>
/// This class specifies the table name and provides a collection of column mappings
/// that associate fields in the <see cref="ArgumentRecord"/> class with corresponding
/// database table columns. It is used to define how data is serialized and deserialized
/// between domain objects and database records.
/// </remarks>
public class ArgumentMap : TableMap<ArgumentRecord>
{
    private static readonly RungMap RungMap = new();
    private static readonly InstructionMap InstructionMap = new();

    /// <inheritdoc />
    public override string TableName => "argument";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<ArgumentRecord>> Columns =>
    [
        ColumnMap<ArgumentRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<ArgumentRecord>.For(r => r.InstructionHash, "instruction_hash"),
        ColumnMap<ArgumentRecord>.For(r => r.Ordinal, "argument_ordinal"),
        ColumnMap<ArgumentRecord>.For(r => r.Argument.Type, "argument_type"),
        ColumnMap<ArgumentRecord>.For(r => r.Argument.ToString(), "argument_text"),
        ColumnMap<ArgumentRecord>.For(r => string.Join('|', r.Argument.Tags), "argument_tags", hashable: false),
        //todo L5Sharp needs to finish this implementation.
        /*ColumnMap<ArgumentRecord>.For(r => string.Join('|', r.Argument.Values), "argument_values", hashable: false),*/
        ColumnMap<ArgumentRecord>.For(ComputeHash, "record_hash", false)
    ];

    /// <inheritdoc />
    public override IEnumerable<ArgumentRecord> GetRecords(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var id = snapshot.SnapshotId;

        var rungs = source.Query<Rung>().Select(r => new RungRecord(id, r));

        return rungs.SelectMany(rung =>
        {
            var rh = RungMap.ComputeHash(rung);
            return rung.Rung.Instructions().SelectMany(x =>
            {
                var ih = InstructionMap.ComputeHash(new InstructionRecord(snapshot.SnapshotId, rh, x));
                return x.Arguments.Select((a, i) => new ArgumentRecord(snapshot.SnapshotId, ih, (byte)i, a));
            });
        });
    }
}

/// <summary>
/// Represents a database record for an instruction argument within a snapshot.
/// </summary>
/// <remarks>
/// This record encapsulates the data necessary to store and retrieve instruction arguments
/// from the database, including their relationship to a snapshot and parent instruction,
/// as well as their position and content.
/// </remarks>
/// <param name="SnapshotId">The identifier of the snapshot to which this argument belongs.</param>
/// <param name="InstructionHash">The hash of the parent instruction that contains this argument.</param>
/// <param name="Ordinal">The zero-based position of this argument within the instruction's argument list.</param>
/// <param name="Argument">The L5Sharp Argument object containing the argument's type, value, and metadata.</param>
public record ArgumentRecord(int SnapshotId, string InstructionHash, byte Ordinal, Argument Argument);