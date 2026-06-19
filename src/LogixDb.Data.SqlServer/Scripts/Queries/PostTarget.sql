IF NOT EXISTS (SELECT 1
               FROM logix.target
               WHERE target_key = @TargetKey)
    BEGIN
        INSERT INTO logix.target (target_key) VALUES (@TargetKey)
    END
