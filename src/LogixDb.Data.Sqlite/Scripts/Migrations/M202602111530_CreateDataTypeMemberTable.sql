CREATE TABLE data_type_member
(
    member_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    type_id            INTEGER NOT NULL,
    member_name        TEXT    NOT NULL,
    member_description TEXT    NULL,
    member_index       INTEGER NOT NULL,
    data_type          TEXT    NULL,
    dimensions         TEXT    NULL,
    radix              TEXT    NULL,
    external_access    TEXT    NULL,
    is_hidden          INTEGER NULL,
    target_name        TEXT    NULL,
    bit_number         INTEGER NULL,
    record_hash        TEXT    NOT NULL,
    FOREIGN KEY (type_id) REFERENCES data_type (type_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_data_type_member_type_id_member_name ON data_type_member (type_id ASC, member_name ASC);
CREATE UNIQUE INDEX IX_data_type_member_type_id_record_hash ON data_type_member (type_id ASC, record_hash ASC);
CREATE INDEX IX_data_type_member_member_name ON data_type_member (member_name ASC);
