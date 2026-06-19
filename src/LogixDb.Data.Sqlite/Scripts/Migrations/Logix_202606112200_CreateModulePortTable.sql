CREATE TABLE module_port
(
    port_id     INTEGER PRIMARY KEY AUTOINCREMENT,
    module_id   INTEGER NOT NULL,
    port_number INTEGER NOT NULL,
    port_type   TEXT    NULL,
    address     TEXT    NULL,
    upstream    INTEGER NULL,
    bus_size    INTEGER NULL,
    record_hash TEXT    NOT NULL,
    FOREIGN KEY (module_id) REFERENCES module (module_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_module_port_module_id_record_hash ON module_port (module_id ASC, record_hash ASC);
