INSERT OR IGNORE INTO rung_reference
(
    rung_id,
    instruction_index,
    argument_index,
    base_reference,
    member_reference
)
SELECT
    r.rung_id,
    t.instruction_index,
    t.argument_index,
    t.base_reference,
    t.member_reference
FROM temp_rung_reference t
     INNER JOIN rung r ON r.record_hash = t.rung_hash;