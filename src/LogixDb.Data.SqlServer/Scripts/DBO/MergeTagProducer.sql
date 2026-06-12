MERGE INTO dbo.tag_producer AS target
USING #temp_tag_producer AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (tag_id,
            produce_count,
            send_event_trigger,
            unicast_permitted,
            maximum_rpi,
            minimum_rpi,
            default_rpi,
            record_hash)
    VALUES ((SELECT tag_id FROM dbo.tag WHERE record_hash = source.tag_hash),
            source.produce_count,
            source.send_event_trigger,
            source.unicast_permitted,
            source.maximum_rpi,
            source.minimum_rpi,
            source.default_rpi,
            source.record_hash);
