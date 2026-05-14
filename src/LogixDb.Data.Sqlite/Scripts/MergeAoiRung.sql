INSERT OR IGNORE INTO aoi_rung (aoi_id,
                                routine_name,
                                rung_number,
                                rung_text,
                                rung_comment,
                                record_hash)
SELECT (SELECT aoi_id FROM aoi WHERE record_hash = t.aoi_hash),
       t.routine_name,
       t.rung_number,
       t.rung_text,
       t.rung_comment,
       t.record_hash
FROM temp_aoi_rung t;
