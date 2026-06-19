DELETE
FROM logix.target_version
WHERE target_id = (SELECT target_id FROM logix.target WHERE target_key = @TargetKey)
  AND import_date < @BeforeDate
