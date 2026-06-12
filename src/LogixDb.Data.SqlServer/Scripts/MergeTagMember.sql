MERGE INTO dbo.tag_member AS target
USING #temp_tag_member AS source
ON target.tag_id = (SELECT tag_id
                    FROM dbo.tag
                    WHERE record_hash = source.tag_hash)
    AND target.tag_name = source.tag_name
WHEN NOT MATCHED THEN
    INSERT (tag_id,
            tag_name,
            parent_name,
            member_name,
            data_type)
    VALUES ((SELECT tag_id FROM dbo.tag WHERE record_hash = source.tag_hash),
            source.tag_name,
            source.parent_name,
            source.member_name,
            source.data_type);
