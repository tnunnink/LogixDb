MERGE INTO dbo.program AS target
USING #temp_program AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (task_name,
            folder_name,
            program_name,
            program_description,
            program_type,
            main_routine,
            fault_routine,
            is_disabled,
            is_folder,
            has_test_edits,
            record_hash)
    VALUES (source.task_name,
            source.folder_name,
            source.program_name,
            source.program_description,
            source.program_type,
            source.main_routine,
            source.fault_routine,
            source.is_disabled,
            source.is_folder,
            source.has_test_edits,
            source.record_hash);

INSERT INTO dbo.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT program_id FROM dbo.program WHERE record_hash = t.record_hash),
       (SELECT component_id FROM dbo.target_component WHERE component_name = 'program')
FROM #temp_program t;
