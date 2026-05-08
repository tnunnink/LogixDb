INSERT INTO tag_member (tag_id,
                        parent_id,
                        tag_name,
                        member_name,
                        data_type,
                        record_hash)
SELECT (SELECT tag_id FROM tag WHERE record_hash = t.tag_id),
       (SELECT member_id
        FROM tag_member
        WHERE record_hash = t.parent_id
          AND tag_id = (SELECT tag_id FROM tag WHERE record_hash = t.tag_id)),
       tag_name,
       member_name,
       data_type,
       record_hash
FROM temp_tag_member t
ON CONFLICT (tag_id, parent_id, tag_name) DO NOTHING;
