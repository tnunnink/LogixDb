MERGE INTO logix.task AS target
USING #temp_task AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (task_name,
            task_description,
            task_type,
            priority,
            scan_rate,
            watchdog,
            is_inhibited,
            disable_outputs,
            event_trigger,
            event_tag,
            enable_timeout,
            record_hash)
    VALUES (source.task_name,
            source.task_description,
            source.task_type,
            source.priority,
            source.scan_rate,
            source.watchdog,
            source.is_inhibited,
            source.disable_outputs,
            source.event_trigger,
            source.event_tag,
            source.enable_timeout,
            source.record_hash);

INSERT INTO logix.target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT task_id FROM logix.task WHERE record_hash = t.record_hash),
       (SELECT component_id FROM logix.target_component WHERE component_name = 'task')
FROM #temp_task t;
