CREATE TABLE tag
(
    tag_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    program_name    TEXT    NULL,
    tag_name        TEXT    NOT NULL,
    tag_description TEXT    NULL,
    data_type       TEXT    NULL,
    dimensions      TEXT    NULL,
    radix           TEXT    NULL,
    external_access TEXT    NULL,
    opcua_access    TEXT    NULL,
    is_constant     INTEGER NULL,
    tag_usage       TEXT    NULL,
    tag_type        TEXT    NULL,
    alias_for       TEXT    NULL,
    content_hash    TEXT    NOT NULL,
    record_hash     TEXT    NOT NULL
);

CREATE UNIQUE INDEX IX_tag_record_hash ON tag (record_hash ASC);
CREATE INDEX IX_tag_program_name_tag_name ON tag (program_name ASC, tag_name ASC);
CREATE INDEX IX_tag_tag_name ON tag (tag_name ASC);
CREATE INDEX IX_tag_data_type ON tag (data_type ASC);
