INSERT OR IGNORE INTO argument (rung_id,
                                instruction_index,
                                argument_index,
                                argument_type,
                                argument_text)
SELECT t.rung_id,
       t.instruction_index,
       t.argument_index,
       t.argument_type,
       t.argument_text
FROM temp_argument t
         INNER JOIN rung r ON r.rung_id = t.rung_id;