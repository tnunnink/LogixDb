namespace LogixDb.Data.Maps;

public class RungReferenceMap : TableMap<ReferenceRecord>
{
    /// <inheritdoc />
    protected override string TableName => "rung_reference";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<ReferenceRecord>> Columns =>
    [
        ColumnMap<ReferenceRecord>.For(r => r.RungHash, "rung_hash"),
        ColumnMap<ReferenceRecord>.For(r => r.InstructionIndex, "instruction_index"),
        ColumnMap<ReferenceRecord>.For(r => r.ArgumentIndex, "argument_index"),
        ColumnMap<ReferenceRecord>.For(r => r.ReferenceName, "reference_name")
    ];
}

public record ReferenceRecord(
    string RungHash,
    int InstructionIndex,
    byte ArgumentIndex,
    string ReferenceName
);