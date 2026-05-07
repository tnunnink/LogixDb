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
                  record_hash,
                  source_hash)
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
       record_hash,
       source_hash
FROM temp_task
ON CONFLICT (source_hash) DO NOTHING;

INSERT INTO target_version_map (version_id, component_id, component_type)
SELECT @VersionId, (SELECT task_id FROM task WHERE source_hash = t.source_hash), 'task'
FROM temp_task t;