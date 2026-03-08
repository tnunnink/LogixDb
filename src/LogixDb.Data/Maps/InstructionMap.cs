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
public class InstructionMap : TableMap<InstructionRecord>
{
    /// <inheritdoc />
    public override string TableName => "instruction";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<InstructionRecord>> Columns =>
    [
        ColumnMap<InstructionRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<InstructionRecord>.For(r => r.RungHash, "rung_hash"),
        ColumnMap<InstructionRecord>.For(x => x.Instruction.ToString(), "instruction_text"),
        ColumnMap<InstructionRecord>.For(x => x.Instruction.Key, "instruction_key", hashable: false),
        ColumnMap<InstructionRecord>.For(x => x.Instruction.IsDesctructive, "is_destructive", hashable: false),
        ColumnMap<InstructionRecord>.For(x => x.Instruction.IsNative, "is_native", hashable: false),
        ColumnMap<InstructionRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a record containing detailed information about an instruction as stored in the Logix system.
/// Encapsulates data specific to an individual instruction, including metadata and structural identifiers.
/// </summary>
/// <remarks>
/// This record is used within the database mapping layer to provide a strongly-typed representation
/// of an instruction entity. It facilitates the persistence and retrieval of instruction-related data,
/// including the snapshot identifier, the rung hash, and the associated instruction details.
/// </remarks>
public record InstructionRecord(int SnapshotId, string RungHash, Instruction Instruction);