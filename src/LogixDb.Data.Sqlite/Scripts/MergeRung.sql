INSERT OR IGNORE INTO rung
(
    rung_id,
    routine_id,
    rung_number,
    rung_text,
    rung_comment,
    code_hash,
    record_hash
)
SELECT
    t.rung_id,
    (SELECT routine_id FROM routine WHERE record_hash = t.routine_hash),
    t.rung_number,
    t.rung_text,
    t.rung_comment,
    t.code_hash,
    t.record_hash
FROM temp_rung t;
