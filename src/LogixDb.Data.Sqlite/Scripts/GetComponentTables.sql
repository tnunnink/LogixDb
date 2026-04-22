SELECT name
FROM sqlite_master
WHERE type = 'table'
  AND sql LIKE '%instance_id%'
  AND name not in ('target_instance')