MERGE INTO dbo.tag_consumer AS target
USING #temp_tag_consumer AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (tag_id,
            producer,
            remote_tag,
            remote_instance,
            rpi,
            unicast,
            record_hash)
    VALUES ((SELECT tag_id FROM dbo.tag WHERE record_hash = source.tag_hash),
            source.producer,
            source.remote_tag,
            source.remote_instance,
            source.rpi,
            source.unicast,
            source.record_hash);
