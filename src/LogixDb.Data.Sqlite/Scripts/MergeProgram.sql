INSERT OR IGNORE INTO program (program_name,
                               task_name,
                               folder_name,
                               program_description,
                               program_type,
                               main_routine,
                               fault_routine,
                               is_disabled,
                               is_folder,
                               has_test_edits,
                               record_hash)
SELECT t.program_name,
       t.task_name,
       t.folder_name,
       t.program_description,
       t.program_type,
       t.main_routine,
       t.fault_routine,
       t.is_disabled,
       t.is_folder,
       t.has_test_edits,
       t.record_hash
FROM temp_program t;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT program_id FROM program WHERE record_hash = t.record_hash),
       (SELECT component_id FROM component WHERE component_name = 'program')
FROM temp_program t;