MERGE INTO dbo.aoi_rung AS target
USING #temp_aoi_rung AS source
ON target.aoi_id = (SELECT aoi_id FROM dbo.aoi WHERE record_hash = source.aoi_id)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT (aoi_id,
            routine_name,
            rung_number,
            rung_text,
            rung_comment,
            record_hash)
    VALUES ((SELECT aoi_id FROM dbo.aoi WHERE record_hash = source.aoi_id),
            source.routine_name,
            source.rung_number,
            source.rung_text,
            source.rung_comment,
            source.record_hash);
