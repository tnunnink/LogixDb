INSERT INTO tag_consumer (tag_id,
                          producer,
                          remote_tag,
                          remote_instance,
                          rpi,
                          unicast,
                          record_hash)
SELECT (SELECT tag_id FROM tag WHERE record_hash = t.tag_id),
       t.producer,
       t.remote_tag,
       t.remote_instance,
       t.rpi,
       t.unicast,
       t.record_hash
FROM temp_tag_consumer t
ON CONFLICT (tag_id, record_hash) DO NOTHING;
