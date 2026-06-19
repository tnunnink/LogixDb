INSERT INTO tag_value (version_id, member_id, tag_value)
SELECT
    @VersionId,
    tm.member_id,
    t.tag_value
FROM temp_tag_value t
     JOIN tag tg ON tg.record_hash = t.tag_hash
     JOIN tag_member tm ON tm.tag_id = tg.tag_id AND tm.member_path = t.member_path;
