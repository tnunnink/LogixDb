INSERT INTO tag (program_id,
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
SELECT (SELECT program_id FROM program WHERE record_hash = t.program_id),
       t.tag_name,
       t.data_type,
       t.dimensions,
       t.radix,
       t.external_access,
       t.opcua_access,
       t.is_constant,
       t.tag_usage,
       t.tag_type,
       t.alias_for,
       t.record_hash
FROM temp_tag t
ON CONFLICT (program_id, record_hash) DO NOTHING;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT tag_id
        FROM tag
        WHERE record_hash = t.record_hash
          AND program_id = (SELECT program_id FROM program where record_hash = t.program_id)),
       (select component_id FROM component WHERE component_name = 'tag')
FROM temp_tag t;
