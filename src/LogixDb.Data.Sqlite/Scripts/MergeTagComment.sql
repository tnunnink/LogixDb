INSERT OR IGNORE INTO tag_member_comment (tag_id,
                                   member_path,
                                   comment,
                                   record_hash)
SELECT (SELECT tag_id FROM tag WHERE record_hash = t.tag_hash),
       t.member_path,
       t.comment,
       t.record_hash
FROM temp_tag_member_comment t;