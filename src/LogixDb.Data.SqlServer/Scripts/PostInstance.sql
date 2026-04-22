INSERT INTO target_instance (version_id, restored_on, restored_by)
OUTPUT INSERTED.instance_id
VALUES (@VersionId, @RestoredOn, @RestoredBy)
