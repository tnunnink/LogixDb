MERGE INTO dbo.data_type AS target
USING #temp_data_type AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (type_name,
            type_description,
            type_class,
            type_family,
            record_hash)
    VALUES (source.type_name,
            source.type_description,
            source.type_class,
            source.type_family,
            source.record_hash);

INSERT INTO dbo.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT type_id FROM dbo.data_type WHERE record_hash = t.record_hash),
       (SELECT component_id FROM dbo.target_component WHERE component_name = 'data_type')
FROM #temp_data_type t;
