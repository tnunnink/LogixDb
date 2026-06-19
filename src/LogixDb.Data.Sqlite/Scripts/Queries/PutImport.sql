INSERT INTO import (import_id, import_status, source_type, file_type, file_name)
VALUES (@ImportId, @ImportStatus, @SourceType, @FileType, @FileName)
ON CONFLICT(import_id) DO UPDATE SET import_status = excluded.import_status;