MERGE INTO dbo.program AS target
USING #temp_program AS source
ON target.task_id = (SELECT task_id FROM dbo.task WHERE record_hash = source.task_id)
    AND target.folder_id = (SELECT program_id FROM dbo.program WHERE record_hash = source.folder_id)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (task_id,
            folder_id,
            program_name,
            program_description,
            program_type,
            main_routine,
            fault_routine,
            is_disabled,
            is_folder,
            has_test_edits,
            record_hash)
    VALUES ((SELECT task_id FROM dbo.task WHERE record_hash = source.task_id),
            (SELECT program_id FROM dbo.program WHERE record_hash = source.folder_id),
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
       (SELECT program_id
        FROM dbo.program
        WHERE record_hash = t.record_hash
          AND task_id = (SELECT task_id FROM dbo.task WHERE record_hash = t.task_id)
          AND folder_id = (SELECT program_id FROM dbo.program WHERE record_hash = t.folder_id)),
       (SELECT component_id FROM dbo.component WHERE component_name = 'program')
FROM #temp_program t;
