MERGE INTO dbo.tag_comment AS target
USING #temp_tag_comment AS source
ON target.tag_id = (SELECT tag_id
                    FROM dbo.tag
                    WHERE record_hash = source.tag_hash)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (tag_id,
            tag_name,
            tag_comment,
            record_hash)
    VALUES ((SELECT tag_id FROM tag WHERE record_hash = source.tag_hash),
            source.tag_name,
            source.tag_comment,
            source.record_hash);
