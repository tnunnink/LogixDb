INSERT OR IGNORE INTO rung
(
    routine_name,
    rung_number,
    rung_text,
    rung_comment,
    code_hash,
    record_hash
)
SELECT
    t.routine_name,
    t.rung_number,
    t.rung_text,
    t.rung_comment,
    t.code_hash,
    t.record_hash
FROM temp_rung t;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT
    @VersionId,
    (SELECT rung_id FROM rung WHERE record_hash = t.record_hash),
    (SELECT component_id FROM target_component WHERE component_name = 'rung')
FROM temp_rung t;
