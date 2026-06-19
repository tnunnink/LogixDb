MERGE INTO logix.tag_member_comment AS target
USING #temp_tag_member_comment AS source
ON target.tag_id = (SELECT tag_id FROM logix.tag WHERE record_hash = source.tag_hash)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (tag_id,
            member_path,
            comment,
            record_hash)
    VALUES ((SELECT tag_id FROM logix.tag WHERE record_hash = source.tag_hash),
            source.member_path,
            source.comment,
            source.record_hash);