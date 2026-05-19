INSERT OR IGNORE INTO code_reference (rung_id,
                                     instruction_index,
                                     argument_index,
                                      reference_name)
SELECT t.rung_id,
       t.instruction_index,
       t.argument_index,
       t.reference_name
FROM temp_code_reference t
         INNER JOIN rung r ON r.rung_id = t.rung_id;