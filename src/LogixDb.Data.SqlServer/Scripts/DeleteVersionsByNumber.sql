DELETE FROM target_version 
WHERE target_id = (SELECT target_id FROM target WHERE target_key = @TargetKey)
AND version_number < @VersionNumber
