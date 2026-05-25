namespace LogixDb.Data.Maps;

public class RungArgumentMap : TableMap<ArgumentRecord>
{
    /// <inheritdoc />
    protected override string TableName => "rung_argument";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<ArgumentRecord>> Columns =>
    [
        ColumnMap<ArgumentRecord>.For(r => r.RungId, "rung_id"),
        ColumnMap<ArgumentRecord>.For(r => r.InstructionIndex, "instruction_index"),
        ColumnMap<ArgumentRecord>.For(r => r.ArgumentIndex, "argument_index"),
        ColumnMap<ArgumentRecord>.For(r => r.ArgumentType, "argument_type"),
        ColumnMap<ArgumentRecord>.For(r => r.ArgumentText, "argument_text"),
        ColumnMap<ArgumentRecord>.RecordHash(this)
    ];
}

public record ArgumentRecord(
    Guid RungId,
    int InstructionIndex,
    byte ArgumentIndex,
    string ArgumentType,
    string ArgumentText
);