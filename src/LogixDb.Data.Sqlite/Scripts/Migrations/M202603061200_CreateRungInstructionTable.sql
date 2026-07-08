CREATE TABLE rung_instruction
(
    rung_id           INTEGER NOT NULL,
    instruction_index INTEGER NOT NULL,
    instruction_text  TEXT    NOT NULL,
    instruction_key   TEXT    NOT NULL,
    is_conditional    INTEGER NOT NULL,
    is_native         INTEGER NOT NULL,
    record_hash       TEXT    NOT NULL,
    FOREIGN KEY (rung_id) REFERENCES rung (rung_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_rung_instruction_rung_id_instruction_index
    ON rung_instruction (rung_id ASC, instruction_index ASC);

CREATE INDEX IX_rung_instruction_instruction_key
    ON rung_instruction (instruction_key ASC);

CREATE INDEX IX_rung_instruction_record_hash
    ON rung_instruction (record_hash ASC);
