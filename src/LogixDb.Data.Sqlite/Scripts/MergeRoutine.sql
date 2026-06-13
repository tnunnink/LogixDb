INSERT OR IGNORE INTO routine
(
    container_name,
    routine_name,
    routine_description,
    routine_type,
    is_definition,
    content_hash,
    record_hash
)
SELECT
    container_name,
    routine_name,
    routine_description,
    routine_type,
    is_definition,
    content_hash,
    record_hash
FROM temp_routine t;

INSERT INTO target_version_map
(
    version_id, record_id, component_id
)
SELECT
    @VersionId,
    (SELECT routine_id FROM routine WHERE record_hash = t.record_hash),
    (SELECT component_id FROM target_component WHERE component_name = 'routine')
FROM temp_routine t;
