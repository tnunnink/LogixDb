MERGE INTO dbo.routine AS target
USING #temp_routine AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (program_name,
            routine_name,
            routine_description,
            routine_type,
            content_hash,
            record_hash)
    VALUES (source.program_name,
            source.routine_name,
            source.routine_description,
            source.routine_type,
            source.content_hash,
            source.record_hash);

INSERT INTO dbo.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT routine_id FROM dbo.routine WHERE record_hash = t.record_hash),
       (SELECT component_id FROM dbo.target_component WHERE component_name = 'routine')
FROM #temp_routine t;
