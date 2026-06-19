CREATE TABLE rung
(
    rung_id        INTEGER PRIMARY KEY,
    container_name TEXT    NOT NULL,
    routine_name   TEXT    NOT NULL,
    rung_number    INTEGER NOT NULL,
    rung_text      TEXT    NULL,
    rung_comment   TEXT    NULL,
    code_hash      TEXT    NULL,
    record_hash    TEXT    NOT NULL
);

CREATE UNIQUE INDEX IX_rung_record_hash ON rung (record_hash ASC);
CREATE INDEX IX_rung_container_name_routine_name_rung_number ON rung (container_name ASC, routine_name ASC, rung_number ASC);
CREATE INDEX IX_rung_routine_name_rung_number ON rung (routine_name ASC, rung_number ASC);
CREATE INDEX IX_rung_code_hash ON rung (code_hash ASC);
