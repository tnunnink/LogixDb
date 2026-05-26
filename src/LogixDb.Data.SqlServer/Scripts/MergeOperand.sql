MERGE INTO dbo.operand AS target
USING #temp_operand AS source
ON target.instruction_key = source.instruction_key
    AND target.operand_index = source.operand_index
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (instruction_key,
            operand_index,
            operand_name,
            operand_type,
            operand_description,
            is_destructive,
            record_hash)
    VALUES (source.instruction_key,
            source.operand_index,
            source.operand_name,
            source.operand_type,
            source.operand_description,
            source.is_destructive,
            source.record_hash);
