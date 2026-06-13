INSERT INTO rung_argument
(
    rung_id,
    instruction_index,
    argument_index,
    argument_type,
    argument_text,
    record_hash
)
SELECT
    t.rung_id,
    t.instruction_index,
    t.argument_index,
    t.argument_type,
    t.argument_text,
    t.record_hash
FROM temp_rung_argument t
     INNER JOIN rung r ON r.rung_id = t.rung_id;