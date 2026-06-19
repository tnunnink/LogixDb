MERGE INTO logix.aoi AS target
USING #temp_aoi AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (aoi_name,
            aoi_description,
            aoi_revision,
            aoi_revision_extension,
            aoi_revision_note,
            aoi_vendor,
            aoi_help_text,
            created_date,
            created_by,
            edited_date,
            edited_by,
            software_revision,
            execute_pre_scan,
            execute_post_scan,
            execute_enable_in_false,
            is_encrypted,
            signature_id,
            signature_timestamp,
            component_class,
            content_hash,
            record_hash)
    VALUES (source.aoi_name,
            source.aoi_description,
            source.aoi_revision,
            source.aoi_revision_extension,
            source.aoi_revision_note,
            source.aoi_vendor,
            source.aoi_help_text,
            source.created_date,
            source.created_by,
            source.edited_date,
            source.edited_by,
            source.software_revision,
            source.execute_pre_scan,
            source.execute_post_scan,
            source.execute_enable_in_false,
            source.is_encrypted,
            source.signature_id,
            source.signature_timestamp,
            source.component_class,
            source.content_hash,
            source.record_hash);

INSERT INTO logix.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT aoi_id FROM logix.aoi WHERE record_hash = t.record_hash),
       (SELECT component_id FROM logix.target_component WHERE component_name = 'aoi')
FROM #temp_aoi t;
