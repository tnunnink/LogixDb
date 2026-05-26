IF NOT EXISTS (SELECT 1
               FROM dbo.target
               WHERE target_key = @TargetKey)
    BEGIN
        INSERT INTO dbo.target (target_key) VALUES (@TargetKey)
    END
