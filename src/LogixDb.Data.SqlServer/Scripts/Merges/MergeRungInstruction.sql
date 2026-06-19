MERGE INTO dbo.rung_instruction AS target
USING #temp_rung_instruction AS source
ON target.rung_id = (SELECT rung_id FROM dbo.rung WHERE record_hash = source.rung_hash)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT
    (
        rung_id,
        instruction_index,
        instruction_text,
        instruction_key,
        is_conditional,
        is_native,
        record_hash
    )
    VALUES
    (
        (SELECT rung_id FROM dbo.rung WHERE record_hash = source.rung_hash),
        source.instruction_index,
        source.instruction_text,
        source.instruction_key,
        source.is_conditional,
        source.is_native,
        source.record_hash
    );