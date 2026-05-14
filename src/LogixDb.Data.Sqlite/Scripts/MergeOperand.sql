INSERT OR IGNORE INTO operand (instruction_key,
                               operand_index,
                               operand_name,
                               operand_type,
                               operand_description,
                               is_destructive,
                               record_hash)
SELECT t.instruction_key,
       t.operand_index,
       t.operand_name,
       t.operand_type,
       t.operand_description,
       t.is_destructive,
       t.record_hash
FROM temp_operand t;
