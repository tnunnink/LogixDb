INSERT INTO dbo.rung_instruction (rung_id,
                                  instruction_index,
                                  instruction_text,
                                  instruction_key,
                                  is_conditional,
                                  is_native,
                                  record_hash)
SELECT r.rung_id,
       t.instruction_index,
       t.instruction_text,
       t.instruction_key,
       t.is_conditional,
       t.is_native,
       t.record_hash
FROM #temp_rung_instruction t
         INNER JOIN dbo.rung r ON r.record_hash = t.rung_hash;