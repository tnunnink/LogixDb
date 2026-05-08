INSERT INTO program (task_id,
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
SELECT (SELECT task_id FROM task WHERE record_hash = t.task_id),
       (SELECT program_id FROM program WHERE record_hash = t.folder_id),
       t.program_name,
       t.program_description,
       t.program_type,
       t.main_routine,
       t.fault_routine,
       t.is_disabled,
       t.is_folder,
       t.has_test_edits,
       t.record_hash
FROM temp_program t
ON CONFLICT (task_id, folder_id, record_hash) DO NOTHING;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT program_id
        FROM program
        WHERE record_hash = t.record_hash
          AND task_id = (SELECT task_id FROM task WHERE record_hash = t.task_id)
          AND folder_id = (SELECT program_id FROM program WHERE record_hash = t.folder_id)),
       (SELECT component_id FROM component WHERE component_name = 'program')
FROM temp_program t;