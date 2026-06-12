INSERT OR IGNORE INTO tag (program_name,
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
SELECT t.program_name,
       t.tag_name,
       t.tag_description,
       t.data_type,
       t.dimensions,
       t.radix,
       t.external_access,
       t.opcua_access,
       t.is_constant,
       t.tag_usage,
       t.tag_type,
       t.alias_for,
       t.content_hash,
       t.record_hash
FROM temp_tag t;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT tag_id FROM tag WHERE record_hash = t.record_hash),
       (select component_id FROM target_component WHERE component_name = 'tag')
FROM temp_tag t;
