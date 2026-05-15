INSERT INTO tag_value (version_id, member_id, tag_value)
SELECT version_id,
       (SELECT member_id
        FROM tag_member
        WHERE tag_id = (SELECT tag_id FROM tag where record_hash = t.tag_hash)
          AND member_name = t.tag_name),
       tag_value
FROM temp_tag_value t;
