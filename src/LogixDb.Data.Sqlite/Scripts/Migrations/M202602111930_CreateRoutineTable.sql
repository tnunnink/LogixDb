CREATE TABLE routine
(
    routine_id          INTEGER PRIMARY KEY AUTOINCREMENT,
    container_name      TEXT    NOT NULL,
    routine_name        TEXT    NOT NULL,
    routine_description TEXT    NULL,
    routine_type        TEXT    NULL,
    is_definition       INTEGER NOT NULL,
    record_hash         TEXT    NOT NULL
);

CREATE UNIQUE INDEX IX_routine_record_hash ON routine (record_hash ASC);
CREATE INDEX IX_routine_container_name_routine_name ON routine (container_name ASC, routine_name ASC);
CREATE INDEX IX_routine_routine_name ON routine (routine_name ASC);
