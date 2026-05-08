MERGE INTO dbo.tag AS target
USING #temp_tag AS source
ON target.program_id = (SELECT program_id FROM dbo.program WHERE record_hash = source.program_id)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (program_id,
            tag_name,
            data_type,
            dimensions,
            radix,
            external_access,
            opcua_access,
            is_constant,
            tag_usage,
            tag_type,
            alias_for,
            record_hash)
    VALUES ((SELECT program_id FROM dbo.program WHERE record_hash = source.program_id),
            source.tag_name,
            source.data_type,
            source.dimensions,
            source.radix,
            source.external_access,
            source.opcua_access,
            source.is_constant,
            source.tag_usage,
            source.tag_type,
            source.alias_for,
            source.record_hash);

INSERT INTO dbo.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT tag_id
        FROM dbo.tag
        WHERE record_hash = t.record_hash
          AND program_id = (SELECT program_id FROM dbo.program WHERE record_hash = t.program_id)),
       (SELECT component_id FROM dbo.component WHERE component_name = 'tag')
FROM #temp_tag t;
