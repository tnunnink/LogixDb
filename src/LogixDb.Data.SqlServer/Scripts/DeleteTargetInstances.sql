DELETE FROM target_instance 
WHERE version_id IN (SELECT version_id FROM target_version s JOIN target t ON t.target_id = s.target_id WHERE t.target_key = @TargetKey)
