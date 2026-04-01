using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents the database table mapping for the <see cref="Instruction"/> type in the Logix system.
/// Defines the table structure, including the table name and column mappings, used for persisting
/// and retrieving instruction data to and from the database.
/// </summary>
/// <remarks>
/// This class provides a schema definition for the "instruction" table, mapping the properties
/// of an <see cref="Instruction"/> instance to specific database columns. It extends the
/// functionality provided by the <see cref="TableMap{T}"/> base class.
/// </remarks>
/// <seealso cref="TableMap{T}"/>
internal class InstructionMap : TableMap<InstructionRecord>
{
    /// <inheritdoc />
    protected override string TableName => "instruction";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<InstructionRecord>> Columns =>
    [
        ColumnMap<InstructionRecord>.For(r => r.InstructionId, "instruction_id", hashable: false),
        ColumnMap<InstructionRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<InstructionRecord>.For(r => r.RungId, "rung_id"),
        ColumnMap<InstructionRecord>.For(x => x.Index, "instruction_index"),
        ColumnMap<InstructionRecord>.For(x => x.Instruction.ToString(), "instruction_text"),
        ColumnMap<InstructionRecord>.For(x => x.Instruction.Key, "instruction_key", hashable: false),
        ColumnMap<InstructionRecord>.For(x => x.Instruction.IsConditional, "is_conditional", hashable: false),
        ColumnMap<InstructionRecord>.For(x => x.Instruction.IsNative, "is_native", hashable: false),
        ColumnMap<InstructionRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a record containing detailed information about an instruction as stored in the Logix system.
/// Encapsulates data specific to an individual instruction, including metadata and structural identifiers.
/// </summary>
internal record InstructionRecord(int SnapshotId, Guid RungId, short Index, Instruction Instruction)
{
    public Guid InstructionId { get; } = Guid.NewGuid();
}