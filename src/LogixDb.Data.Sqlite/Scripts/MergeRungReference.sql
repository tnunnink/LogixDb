INSERT OR IGNORE INTO rung_reference (rung_id,
                                     instruction_index,
                                     argument_index,
                                      reference_name)
SELECT t.rung_id,
       t.instruction_index,
       t.argument_index,
       t.reference_name
FROM temp_rung_reference t
         INNER JOIN rung r ON r.rung_id = t.rung_id;