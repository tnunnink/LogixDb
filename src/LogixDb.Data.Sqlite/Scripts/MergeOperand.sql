INSERT OR IGNORE INTO operand (instruction_key,
                               operand_index,
                               operand_name,
                               operand_type,
                               operand_description,
                               is_destructive,
                               is_native,
                               record_hash)
SELECT t.instruction_key,
       t.operand_index,
       t.operand_name,
       t.operand_type,
       t.operand_description,
       t.is_destructive,
       t.is_native,
       t.record_hash
FROM temp_operand t;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT operand_id FROM operand WHERE record_hash = t.record_hash),
       (SELECT component_id FROM target_component WHERE component_name = 'operand')
FROM temp_operand t;
