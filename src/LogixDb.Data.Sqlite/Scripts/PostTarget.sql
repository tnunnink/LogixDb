INSERT INTO target (target_key)
VALUES (@TargetKey)
ON CONFLICT(target_key) DO NOTHING;