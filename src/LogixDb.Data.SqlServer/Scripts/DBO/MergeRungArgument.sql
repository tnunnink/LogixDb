INSERT INTO dbo.rung_argument (rung_id,
                               instruction_index,
                               argument_index,
                               argument_type,
                               argument_text,
                               record_hash)
SELECT r.rung_id,
       t.instruction_index,
       t.argument_index,
       t.argument_type,
       t.argument_text,
       t.record_hash
FROM #temp_rung_argument t
         INNER JOIN dbo.rung r ON r.record_hash = t.rung_hash;