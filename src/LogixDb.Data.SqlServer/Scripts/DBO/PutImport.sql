MERGE INTO [import] AS target
USING (SELECT @ImportId AS import_id) AS source
ON (target.import_id = source.import_id)
WHEN MATCHED THEN
    UPDATE SET
        import_status = @ImportStatus
WHEN NOT MATCHED THEN
    INSERT (import_id, import_status, source_type, file_type, file_name)
    VALUES (@ImportId, @Status, @SourceType, @FileType, @FileName);