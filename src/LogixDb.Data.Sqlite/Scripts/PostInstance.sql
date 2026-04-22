INSERT INTO target_instance (version_id, restored_on, restored_by)
VALUES (@VersionId, @ResotredOn, @RestoredBy)
RETURNING instance_id;