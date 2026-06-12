MERGE INTO dbo.rung AS target
USING (
    SELECT t.*, r.routine_id
    FROM #temp_rung t
             INNER JOIN dbo.routine r ON r.record_hash = t.routine_hash
) AS source
ON target.routine_id = source.routine_id
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (rung_id,
            routine_id,
            rung_number,
            rung_text,
            rung_comment,
            code_hash,
            record_hash)
    VALUES (source.rung_id,
            source.routine_id,
            source.rung_number,
            source.rung_text,
            source.rung_comment,
            source.code_hash,
            source.record_hash);
