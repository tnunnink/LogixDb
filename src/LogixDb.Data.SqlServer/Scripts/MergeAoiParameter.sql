MERGE INTO dbo.aoi_parameter AS target
USING #temp_aoi_parameter AS source
ON target.aoi_id = (SELECT aoi_id FROM dbo.aoi WHERE record_hash = source.aoi_hash)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (aoi_id,
            parameter_name,
            parameter_description,
            data_type,
            dimensions,
            radix,
            default_value,
            external_access,
            tag_usage,
            tag_type,
            tag_alias,
            is_visible,
            is_required,
            is_constant,
            record_hash)
    VALUES ((SELECT aoi_id FROM dbo.aoi WHERE record_hash = source.aoi_hash),
            source.parameter_name,
            source.parameter_description,
            source.data_type,
            source.dimensions,
            source.radix,
            source.default_value,
            source.external_access,
            source.tag_usage,
            source.tag_type,
            source.tag_alias,
            source.is_visible,
            source.is_required,
            source.is_constant,
            source.record_hash);
