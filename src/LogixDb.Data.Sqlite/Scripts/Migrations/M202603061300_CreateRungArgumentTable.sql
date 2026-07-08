CREATE TABLE rung_argument
(
    rung_id           INTEGER NOT NULL,
    instruction_index INTEGER NOT NULL,
    argument_index    INTEGER NOT NULL,
    argument_type     TEXT    NOT NULL,
    argument_text     TEXT    NOT NULL,
    record_hash       TEXT    NOT NULL,
    FOREIGN KEY (rung_id) REFERENCES rung (rung_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_rung_argument_rung_id_instruction_index_argument_index
    ON rung_argument (rung_id ASC, instruction_index ASC, argument_index ASC);
