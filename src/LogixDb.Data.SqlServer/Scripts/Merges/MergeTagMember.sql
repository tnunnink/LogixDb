MERGE INTO dbo.tag_member AS target
USING #temp_tag_member AS source
ON target.tag_id = (SELECT tag_id
                    FROM dbo.tag
                    WHERE record_hash = source.tag_hash)
    AND target.member_path = source.member_path
WHEN NOT MATCHED THEN
    INSERT (tag_id,
            member_path,
            parent_path,
            member_name,
            data_type)
    VALUES ((SELECT tag_id FROM dbo.tag WHERE record_hash = source.tag_hash),
            source.member_path,
            source.parent_path,
            source.member_name,
            source.data_type);
