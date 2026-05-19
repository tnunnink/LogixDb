namespace LogixDb.Data.Maps;

public class ReferenceMap : TableMap<ReferenceRecord>
{
    /// <inheritdoc />
    protected override string TableName => "code_reference";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<ReferenceRecord>> Columns =>
    [
        ColumnMap<ReferenceRecord>.For(r => r.RungId, "rung_id"),
        ColumnMap<ReferenceRecord>.For(r => r.InstructionIndex, "instruction_index"),
        ColumnMap<ReferenceRecord>.For(r => r.ArgumentIndex, "argument_index"),
        ColumnMap<ReferenceRecord>.For(r => r.TagName, "reference_name")
    ];
}

public record ReferenceRecord(
    Guid RungId,
    int InstructionIndex,
    byte ArgumentIndex,
    string TagName
);