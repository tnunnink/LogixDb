INSERT INTO data_type_member (type_id,
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
SELECT (SELECT type_id FROM data_type WHERE record_hash = t.type_id),
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
FROM temp_data_type_member t
ON CONFLICT (type_id, record_hash) DO NOTHING; 
