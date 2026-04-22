DELETE
FROM target_instance
WHERE version_id in (
    (SELECT version_id
     FROM target_version
     WHERE target_id = (SELECT target_id FROM target WHERE target_key = @TargetKey)));