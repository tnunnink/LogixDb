MERGE INTO dbo.rung AS target
USING #temp_rung AS source
ON target.routine_id = (SELECT routine_id FROM dbo.routine WHERE record_hash = source.routine_id)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (routine_id,
            rung_number,
            rung_text,
            rung_comment,
            record_hash)
    VALUES ((SELECT routine_id FROM dbo.routine WHERE record_hash = source.routine_id),
            source.rung_number,
            source.rung_text,
            source.rung_comment,
            source.record_hash);

INSERT INTO dbo.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT rung_id
        FROM dbo.rung
        WHERE record_hash = t.record_hash
          AND routine_id = (SELECT routine_id FROM dbo.routine WHERE record_hash = t.routine_id)),
       (SELECT component_id FROM dbo.component WHERE component_name = 'rung')
FROM #temp_rung t;
