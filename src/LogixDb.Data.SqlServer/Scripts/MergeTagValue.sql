INSERT INTO dbo.tag_value (version_id, member_id, tag_value)
SELECT @VersionId,
       tm.member_id,
       t.tag_value
FROM #temp_tag_value t
         JOIN dbo.tag tg ON tg.record_hash = t.tag_hash
         JOIN dbo.tag_member tm ON tm.tag_id = tg.tag_id AND tm.tag_name = t.tag_name;
