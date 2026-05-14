INSERT OR IGNORE INTO instruction (rung_id,
                                   instruction_index,
                                   instruction_text,
                                   instruction_key,
                                   is_conditional,
                                   is_native,
                                   record_hash)
SELECT t.rung_id,
       t.instruction_index,
       t.instruction_text,
       t.instruction_key,
       t.is_conditional,
       t.is_native,
       t.record_hash
FROM temp_instruction t
         INNER JOIN rung r ON r.rung_id = t.rung_id;


