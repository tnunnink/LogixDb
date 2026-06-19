CREATE TABLE import
(
    import_id     TEXT PRIMARY KEY,
    import_status TEXT     NOT NULL,
    source_type   TEXT     NOT NULL,
    file_type     TEXT     NOT NULL,
    file_name     TEXT     NOT NULL,
    posted_on     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IX_import_file_name ON import (file_name ASC);
CREATE INDEX IX_import_posted_on ON import (posted_on DESC);

CREATE TABLE import_log
(
    log_id        INTEGER PRIMARY KEY,
    import_id     TEXT     NOT NULL,
    timestamp     DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    log_severity  TEXT     NOT NULL,
    log_message   TEXT     NOT NULL,
    log_exception TEXT     NULL,
    FOREIGN KEY (import_id) REFERENCES import (import_id) ON DELETE CASCADE
);

CREATE INDEX IX_import_log_import_id ON import_log (import_id ASC);
