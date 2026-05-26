MERGE INTO dbo.rung AS target
USING #temp_rung AS source
ON target.routine_id = (SELECT routine_id
                        FROM dbo.routine
                        WHERE record_hash = source.routine_hash)
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
            (SELECT routine_id FROM dbo.routine WHERE record_hash = source.routine_hash),
            source.rung_number,
            source.rung_text,
            source.rung_comment,
            source.code_hash,
            source.record_hash);
