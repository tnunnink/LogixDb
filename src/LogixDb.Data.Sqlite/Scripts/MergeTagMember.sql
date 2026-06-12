INSERT OR IGNORE INTO tag_member (tag_id,
                                  member_path,
                                  member_name,
                                  parent_name,
                                  data_type)
SELECT (SELECT tag_id FROM tag WHERE record_hash = t.tag_hash),
       t.member_path,
       t.member_name,
       t.parent_name,
       t.data_type
FROM temp_tag_member t;
