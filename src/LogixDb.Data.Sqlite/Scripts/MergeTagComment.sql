INSERT OR IGNORE INTO tag_comment (tag_id,
                                   tag_name,
                                   tag_comment,
                                   record_hash)
SELECT (SELECT tag_id FROM tag WHERE record_hash = t.tag_hash),
       t.tag_name,
       t.tag_comment,
       t.record_hash
FROM temp_tag_comment t;
