MERGE INTO dbo.tag_member AS target
USING #temp_tag_member AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (tag_id,
            parent_id,
            tag_name,
            member_name,
            data_type,
            record_hash)
    VALUES ((SELECT tag_id FROM dbo.tag WHERE record_hash = source.tag_hash),
            (SELECT member_id FROM dbo.tag_member WHERE record_hash = source.parent_hash),
            source.tag_name,
            source.member_name,
            source.data_type,
            source.record_hash);
