MERGE INTO logix.rung AS target
USING #temp_rung AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT
    (
        container_name,
        routine_name,
        rung_number,
        rung_text,
        rung_comment,
        code_hash,
        record_hash
    )
    VALUES
    (
        source.container_name,
        source.routine_name,
        source.rung_number,
        source.rung_text,
        source.rung_comment,
        source.code_hash,
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
    (SELECT rung_id FROM logix.rung WHERE record_hash = t.record_hash),
    (SELECT component_id FROM logix.target_component WHERE component_name = 'rung')
FROM #temp_rung t;