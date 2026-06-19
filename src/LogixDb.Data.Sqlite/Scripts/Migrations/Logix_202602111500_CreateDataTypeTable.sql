CREATE TABLE data_type
(
    type_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    type_name        TEXT NOT NULL,
    type_description TEXT NULL,
    type_class       TEXT NULL,
    type_family      TEXT NULL,
    content_hash     TEXT NOT NULL,
    record_hash      TEXT NOT NULL
);

CREATE UNIQUE INDEX IX_data_type_record_hash ON data_type (record_hash ASC);
CREATE INDEX IX_data_type_type_name ON data_type (type_name ASC);
