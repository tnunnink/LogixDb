CREATE TABLE module
(
    module_id              INTEGER PRIMARY KEY AUTOINCREMENT,
    module_name            TEXT    NOT NULL,
    module_description     TEXT    NULL,
    catalog_number         TEXT    NULL,
    revision               TEXT    NULL,
    vendor_id              INTEGER NULL,
    product_id             INTEGER NULL,
    product_code           INTEGER NULL,
    parent_name            TEXT    NOT NULL,
    parent_port            INTEGER NOT NULL,
    electronic_keying      TEXT    NULL,
    is_inhibited           INTEGER NULL,
    is_major_fault_enabled INTEGER NULL,
    is_safety_enabled      INTEGER NULL,
    config_tag             TEXT    NULL,
    ip_address             TEXT    NULL,
    slot_number            INTEGER NULL,
    content_hash           TEXT    NOT NULL,
    record_hash            TEXT    NOT NULL
);

CREATE UNIQUE INDEX IX_module_record_hash ON module (record_hash ASC);
CREATE INDEX IX_module_module_name ON module (module_name ASC);
CREATE INDEX IX_module_parent_name ON module (parent_name ASC);
