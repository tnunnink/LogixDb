INSERT INTO routine (routine_id,
                     program_id,
                     routine_name,
                     routine_description,
                     routine_type,
                     record_hash,
                     source_hash)
SELECT routine_id,
       (SELECT program_id
        from program
        WHERE source_hash = (SELECT source_hash FROM temp_program WHERE program_id = t.program_id)),
       routine_name,
       routine_description,
       routine_type,
       record_hash,
       source_hash
FROM temp_routine t
ON CONFLICT (program_id, source_hash) DO NOTHING;

INSERT INTO target_version_map (version_id, component_id, component_type)
SELECT @VersionId, routine_id, 'routine'
FROM temp_routine;
