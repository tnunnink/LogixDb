CREATE TABLE target_version
(
    version_id        INTEGER PRIMARY KEY AUTOINCREMENT,
    target_id         INTEGER  NOT NULL,
    version_number    INTEGER  NOT NULL,
    target_type       TEXT     NOT NULL,
    target_name       TEXT     NULL,
    target_count      INTEGER  NULL,
    schema_revision   TEXT     NULL,
    software_revision TEXT     NULL,
    is_partial        INTEGER  NOT NULL,
    import_date       DATETIME NOT NULL,
    import_user       TEXT     NOT NULL,
    import_machine    TEXT     NOT NULL,
    export_date       DATETIME NULL,
    export_options    TEXT     NULL,
    source_hash       TEXT     NOT NULL,
    source_data       BLOB     NOT NULL,
    FOREIGN KEY (target_id) REFERENCES target (target_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE UNIQUE INDEX IX_target_version_target_id_version_number ON target_version (target_id ASC, version_number ASC);
CREATE INDEX IX_target_version_target_id_import_date ON target_version (target_id ASC, import_date DESC);
CREATE INDEX IX_target_version_target_name_version_number ON target_version (target_name ASC, version_number ASC);
