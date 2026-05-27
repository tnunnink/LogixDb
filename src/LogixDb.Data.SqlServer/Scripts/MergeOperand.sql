MERGE INTO dbo.operand AS target
USING #temp_operand AS source
ON target.record_hash = source.record_hash
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

INSERT INTO dbo.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT operand_id FROM dbo.operand WHERE record_hash = t.record_hash),
       (SELECT component_id FROM dbo.target_component WHERE component_name = 'operand')
FROM #temp_operand t;
