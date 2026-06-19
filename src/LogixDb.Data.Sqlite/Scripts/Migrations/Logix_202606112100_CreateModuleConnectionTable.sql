CREATE TABLE module_connection
(
    connection_id                      INTEGER PRIMARY KEY AUTOINCREMENT,
    module_id                          INTEGER NOT NULL,
    connection_name                    TEXT    NULL,
    rpi                                INTEGER NULL,
    connection_type                    TEXT    NULL,
    connection_priority                TEXT    NULL,
    transmission_type                  TEXT    NULL,
    production_trigger                 TEXT    NULL,
    output_redundant_owner             INTEGER NULL,
    unicast                            INTEGER NULL,
    programatically_send_event_trigger INTEGER NULL,
    event_id                           INTEGER NULL,
    input_tag                          TEXT    NULL,
    input_size                         INTEGER NULL,
    input_suffix                       TEXT    NULL,
    output_tag                         TEXT    NULL,
    output_size                        INTEGER NULL,
    output_suffix                      TEXT    NULL,
    connection_path                    TEXT    NULL,
    record_hash                        TEXT    NOT NULL,
    FOREIGN KEY (module_id) REFERENCES module (module_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_module_connection_module_id_record_hash ON module_connection (module_id ASC, record_hash ASC);
