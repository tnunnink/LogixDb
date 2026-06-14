namespace LogixDb.Data.Maps;

public class RungInstructionMap : TableMap<InstructionRecord>
{
    /// <inheritdoc />
    protected override string TableName => "rung_instruction";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<InstructionRecord>> Columns =>
    [
        ColumnMap<InstructionRecord>.For(r => r.RungHash, "rung_hash", hashable: false),
        ColumnMap<InstructionRecord>.For(x => x.Index, "instruction_index"),
        ColumnMap<InstructionRecord>.For(x => x.Text, "instruction_text"),
        ColumnMap<InstructionRecord>.For(x => x.Key, "instruction_key"),
        ColumnMap<InstructionRecord>.For(x => x.IsConditional, "is_conditional"),
        ColumnMap<InstructionRecord>.For(x => x.IsNative, "is_native"),
        ColumnMap<InstructionRecord>.RecordHash(this)
    ];
}

public record InstructionRecord(
    string RungHash,
    short Index,
    string Text,
    string Key,
    bool IsConditional,
    bool IsNative
);