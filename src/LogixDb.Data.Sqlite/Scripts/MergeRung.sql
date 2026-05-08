INSERT INTO rung (routine_id,
                  rung_number,
                  rung_text,
                  rung_comment,
                  record_hash)
SELECT (SELECT routine_id from routine WHERE record_hash = t.routine_id),
       t.rung_number,
       t.rung_text,
       t.rung_comment,
       t.record_hash
FROM temp_rung t
ON CONFLICT (routine_id, record_hash) DO NOTHING;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT rung_id
        FROM rung
        WHERE record_hash = t.record_hash
          AND routine_id = (SELECT routine_id FROM routine WHERE record_hash = t.routine_id)),
       (SELECT component_id FROM component WHERE component_name = 'rung')
FROM temp_rung t;
