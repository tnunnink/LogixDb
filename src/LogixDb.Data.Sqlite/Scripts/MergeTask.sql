INSERT INTO task (task_id,
                  task_name,
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
SELECT task_id,
       task_name,
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
       record_hash
FROM temp_task
ON CONFLICT (record_hash) DO NOTHING;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT task_id FROM task WHERE record_hash = t.record_hash),
       (SELECT component_id FROM component WHERE component_name = 'task')
FROM temp_task t;