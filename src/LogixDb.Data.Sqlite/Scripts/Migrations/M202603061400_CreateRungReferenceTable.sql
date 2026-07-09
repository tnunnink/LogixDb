CREATE TABLE rung_reference
(
    rung_id           INTEGER NOT NULL,
    instruction_index INTEGER NOT NULL,
    argument_index    INTEGER NOT NULL,
    base_reference    TEXT    NOT NULL,
    member_reference  TEXT    NOT NULL,
    FOREIGN KEY (rung_id) REFERENCES rung (rung_id) ON DELETE CASCADE
);

CREATE INDEX IX_rung_reference_rung_id_instruction_index_argument_index
    ON rung_reference (rung_id ASC, instruction_index ASC, argument_index ASC);

CREATE INDEX IX_rung_reference_base_reference
    ON rung_reference (base_reference ASC);
