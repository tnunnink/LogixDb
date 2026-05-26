MERGE INTO dbo.tag_comment AS target
USING #temp_tag_comment AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (member_id,
            tag_name,
            tag_comment,
            record_hash)
    VALUES ((SELECT member_id FROM dbo.tag_member WHERE record_hash = source.member_hash),
            source.tag_name,
            source.tag_comment,
            source.record_hash);
