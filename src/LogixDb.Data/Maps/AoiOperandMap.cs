namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a table mapping for AOI (Add-On Instruction) operands, which defines how operand data
/// is extracted from Logix snapshots and mapped to database records.
/// </summary>
internal class AoiOperandMap : TableMap<AoiOperandRecord>
{
    /// <inheritdoc />
    protected override string TableName => "operand";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<AoiOperandRecord>> Columns =>
    [
        ColumnMap<AoiOperandRecord>.For(r => r.OperandId, "operand_id"),
        ColumnMap<AoiOperandRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<AoiOperandRecord>.For(r => r.Key, "instruction_key"),
        ColumnMap<AoiOperandRecord>.For(r => r.Index, "operand_index"),
        ColumnMap<AoiOperandRecord>.For(r => r.Name, "operand_name"),
        ColumnMap<AoiOperandRecord>.For(r => r.Type, "operand_type"),
        ColumnMap<AoiOperandRecord>.For(r => r.Description, "operand_description"),
        ColumnMap<AoiOperandRecord>.For(r => r.Destructive, "is_destructive")
    ];
}

/// <summary>
/// Represents a record that defines the structure and properties of an operand used in an AOI (Add-On Instruction).
/// This record is part of the LogixDB data mapping system and corresponds to entries in the "operand" table.
/// </summary>
/// <remarks>
/// The AoiOperandRecord is a data structure designed to hold information about individual operands associated
/// with AOI instructions. Each operand has properties such as its unique key, type, format, and other metadata
/// that describe its functionality and behavior in the context of AOI usage.
/// </remarks>
public record AoiOperandRecord(
    int SnapshotId,
    string Key,
    byte Index,
    string Name,
    string Type,
    string? Description,
    bool Destructive)
{
    public Guid OperandId { get; } = Guid.NewGuid();
}