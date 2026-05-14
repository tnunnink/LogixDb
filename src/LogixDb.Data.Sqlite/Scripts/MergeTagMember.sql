INSERT OR IGNORE INTO tag_member (tag_id,
                                  tag_name,
                                  parent_name,
                                  member_name,
                                  data_type)
SELECT (SELECT tag_id FROM tag WHERE record_hash = t.tag_hash),
       t.tag_name,
       t.parent_name,
       t.member_name,
       t.data_type
FROM temp_tag_member t;
