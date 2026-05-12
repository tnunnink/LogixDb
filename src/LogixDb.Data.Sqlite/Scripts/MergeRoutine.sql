INSERT OR IGNORE INTO routine (program_id,
                     routine_name,
                     routine_description,
                     routine_type,
                     record_hash)
SELECT (SELECT program_id from program WHERE record_hash = t.program_id),
       routine_name,
       routine_description,
       routine_type,
       record_hash
FROM temp_routine t;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT routine_id
        FROM routine
        WHERE record_hash = t.record_hash
          AND program_id = (SELECT program_id FROM program WHERE record_hash = t.program_id)),
       (SELECT component_id FROM component WHERE component_name = 'routine')
FROM temp_routine t;
