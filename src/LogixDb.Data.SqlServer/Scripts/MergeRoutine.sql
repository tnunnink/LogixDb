MERGE INTO dbo.routine AS target
USING #temp_routine AS source
ON target.program_id = (SELECT program_id FROM dbo.program WHERE record_hash = source.program_id)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (program_id,
            routine_name,
            routine_description,
            routine_type,
            record_hash)
    VALUES ((SELECT program_id FROM dbo.program WHERE record_hash = source.program_id),
            source.routine_name,
            source.routine_description,
            source.routine_type,
            source.record_hash);

INSERT INTO dbo.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT routine_id
        FROM dbo.routine
        WHERE record_hash = t.record_hash
          AND program_id = (SELECT program_id FROM dbo.program WHERE record_hash = t.program_id)),
       (SELECT component_id FROM dbo.component WHERE component_name = 'routine')
FROM #temp_routine t;
