MERGE INTO dbo.tag AS target
USING #temp_tag AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (program_name,
            tag_name,
            tag_description,
            data_type,
            dimensions,
            radix,
            external_access,
            opcua_access,
            is_constant,
            tag_usage,
            tag_type,
            alias_for,
            content_hash,
            record_hash)
    VALUES (source.program_name,
            source.tag_name,
            source.tag_description,
            source.data_type,
            source.dimensions,
            source.radix,
            source.external_access,
            source.opcua_access,
            source.is_constant,
            source.tag_usage,
            source.tag_type,
            source.alias_for,
            source.content_hash,
            source.record_hash);

INSERT INTO dbo.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT tag_id FROM dbo.tag WHERE record_hash = t.record_hash),
       (SELECT component_id FROM dbo.target_component WHERE component_name = 'tag')
FROM #temp_tag t;
