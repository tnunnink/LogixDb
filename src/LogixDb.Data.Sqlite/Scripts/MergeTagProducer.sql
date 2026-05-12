INSERT OR IGNORE INTO tag_producer (tag_id,
                          produce_count,
                          send_event_trigger,
                          unicast_permitted,
                          maximum_rpi,
                          minimum_rpi,
                          default_rpi,
                          record_hash)
SELECT (SELECT tag_id FROM tag WHERE record_hash = t.tag_id),
       t.produce_count,
       t.send_event_trigger,
       t.unicast_permitted,
       t.maximum_rpi,
       t.minimum_rpi,
       t.default_rpi,
       t.record_hash
FROM temp_tag_producer t;
