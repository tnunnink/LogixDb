namespace LogixDb.Data.Maps;

public class OperandMap : TableMap<OperandRecord>
{
    /// <inheritdoc />
    protected override string TableName => "operand";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<OperandRecord>> Columns =>
    [
        ColumnMap<OperandRecord>.For(r => r.Key, "instruction_key"),
        ColumnMap<OperandRecord>.For(r => r.Index, "operand_index"),
        ColumnMap<OperandRecord>.For(r => r.Name, "operand_name"),
        ColumnMap<OperandRecord>.For(r => r.Type, "operand_type"),
        ColumnMap<OperandRecord>.For(r => r.Description, "operand_description"),
        ColumnMap<OperandRecord>.For(r => r.Destructive, "is_destructive"),
        ColumnMap<OperandRecord>.For(r => r.Native, "is_native"),
        ColumnMap<OperandRecord>.RecordHash(this)
    ];
}

public record OperandRecord(
    string Key,
    byte Index,
    string Name,
    string Type,
    string? Description,
    bool Destructive,
    bool Native = false
);