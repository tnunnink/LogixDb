INSERT OR IGNORE INTO data_type_member (type_name,
                                        member_name,
                                        member_description,
                                        data_type,
                                        dimensions,
                                        radix,
                                        external_access,
                                        is_hidden,
                                        target_name,
                                        bit_number,
                                        record_hash)
SELECT type_name,
       member_name,
       member_description,
       data_type,
       dimensions,
       radix,
       external_access,
       is_hidden,
       target_name,
       bit_number,
       record_hash
FROM temp_data_type_member t;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT controller_id FROM controller WHERE record_hash = t.record_hash),
       (SELECT component_id FROM component WHERE component_name = 'data_type')
FROM temp_data_type_member t;
