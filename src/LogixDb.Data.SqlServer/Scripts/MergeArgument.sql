MERGE INTO dbo.argument AS target
USING #temp_argument AS source
ON target.instruction_id = (SELECT instruction_id FROM dbo.instruction WHERE record_hash = source.instruction_id)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (instruction_id,
            argument_index,
            argument_type,
            argument_text,
            record_hash)
    VALUES ((SELECT instruction_id FROM dbo.instruction WHERE record_hash = source.instruction_id),
            source.argument_index,
            source.argument_type,
            source.argument_text,
            source.record_hash);
