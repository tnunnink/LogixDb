MERGE INTO logix.routine AS target
USING #temp_routine AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT
    (
        container_name,
        routine_name,
        routine_description,
        routine_type,
        is_definition,
        record_hash
    )
    VALUES
        (
            source.container_name,
            source.routine_name,
            source.routine_description,
            source.routine_type,
            source.is_definition,
            source.record_hash
        );

INSERT INTO logix.target_version_map
(
    version_id,
    record_id,
    component_id
)
SELECT
    @VersionId,
    (SELECT routine_id FROM logix.routine WHERE record_hash = t.record_hash),
    (SELECT component_id FROM logix.target_component WHERE component_name = 'routine')
FROM #temp_routine t;
