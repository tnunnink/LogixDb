INSERT INTO tag_comment (member_id,
                         tag_name,
                         tag_comment,
                         record_hash)
SELECT (SELECT member_id FROM tag_member WHERE record_hash = t.member_id),
       t.tag_name,
       t.tag_comment,
       t.record_hash
FROM temp_tag_comment t
ON CONFLICT (member_id, record_hash) DO NOTHING;
