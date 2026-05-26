MERGE INTO dbo.data_type_member AS target
USING #temp_data_type_member AS source
ON target.type_id = (SELECT type_id
                     FROM dbo.data_type
                     WHERE record_hash = source.type_hash)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (type_id,
            member_name,
            member_description,
            member_index,
            data_type,
            dimensions,
            radix,
            external_access,
            is_hidden,
            target_name,
            bit_number,
            record_hash)
    VALUES ((SELECT type_id FROM dbo.data_type WHERE record_hash = source.type_hash),
            source.member_name,
            source.member_description,
            source.member_index,
            source.data_type,
            source.dimensions,
            source.radix,
            source.external_access,
            source.is_hidden,
            source.target_name,
            source.bit_number,
            source.record_hash);
