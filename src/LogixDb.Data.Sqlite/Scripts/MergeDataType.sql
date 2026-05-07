INSERT INTO data_type (type_id,
                       type_name,
                       type_description,
                       type_class,
                       type_family,
                       record_hash,
                       source_hash)
SELECT type_id,
       type_name,
       type_description,
       type_class,
       type_family,
       record_hash,
       source_hash
FROM temp_data_type
ON CONFLICT (source_hash) DO NOTHING;

INSERT INTO target_version_map (version_id, component_id, component_type)
SELECT @VersionId, (SELECT type_id FROM data_type WHERE source_hash = t.source_hash), 'data_type'
FROM temp_data_type t;
