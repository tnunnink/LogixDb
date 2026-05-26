INSERT OR IGNORE INTO data_type (type_name,
                                 type_description,
                                 type_class,
                                 type_family,
                                 content_hash,
                                 record_hash)
SELECT type_name,
       type_description,
       type_class,
       type_family,
       content_hash,
       record_hash
FROM temp_data_type;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT type_id FROM data_type WHERE record_hash = t.record_hash),
       (SELECT component_id FROM target_component WHERE component_name = 'data_type')
FROM temp_data_type t;
