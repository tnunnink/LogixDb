INSERT INTO aoi_rung (aoi_id,
                      routine_name,
                      rung_number,
                      rung_text,
                      rung_comment,
                      record_hash)
SELECT (SELECT aoi_id FROM aoi WHERE record_hash = t.aoi_id),
       t.routine_name,
       t.rung_number,
       t.rung_text,
       t.rung_comment,
       t.record_hash
FROM temp_aoi_rung t
ON CONFLICT (aoi_id, record_hash) DO NOTHING;
