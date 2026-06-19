CREATE TABLE aoi
(
    aoi_id                  INTEGER PRIMARY KEY,
    aoi_name                TEXT     NOT NULL,
    aoi_description         TEXT     NULL,
    aoi_revision            TEXT     NULL,
    aoi_revision_extension  TEXT     NULL,
    aoi_revision_note       TEXT     NULL,
    aoi_vendor              TEXT     NULL,
    aoi_help_text           TEXT     NULL,
    created_date            DATETIME NULL,
    created_by              TEXT     NULL,
    edited_date             DATETIME NULL,
    edited_by               TEXT     NULL,
    software_revision       TEXT     NULL,
    execute_pre_scan        INTEGER  NULL,
    execute_post_scan       INTEGER  NULL,
    execute_enable_in_false INTEGER  NULL,
    is_encrypted            INTEGER  NULL,
    signature_id            TEXT     NULL,
    signature_timestamp     DATETIME NULL,
    component_class         TEXT     NULL,
    content_hash            TEXT     NOT NULL,
    record_hash             TEXT     NOT NULL
);

CREATE UNIQUE INDEX IX_aoi_record_hash ON aoi (record_hash ASC);
CREATE INDEX IX_aoi_aoi_name ON aoi (aoi_name ASC);
