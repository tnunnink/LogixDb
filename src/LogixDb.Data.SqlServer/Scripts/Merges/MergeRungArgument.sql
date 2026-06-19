MERGE INTO dbo.rung_argument AS target
USING #temp_rung_argument AS source
ON target.rung_id = (SELECT rung_id FROM dbo.rung WHERE record_hash = source.rung_hash)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT
    (
        rung_id,
        instruction_index,
        argument_index,
        argument_type,
        argument_text,
        record_hash
    )
    VALUES
        (
            (SELECT rung_id FROM dbo.rung WHERE record_hash = source.rung_hash),
            source.instruction_index,
            source.argument_index,
            source.argument_type,
            source.argument_text,
            source.record_hash
        );