CREATE TABLE operand
(
    operand_id          INTEGER PRIMARY KEY,
    instruction_key     TEXT    NOT NULL,
    operand_index       INTEGER NOT NULL,
    operand_name        TEXT    NOT NULL,
    operand_type        TEXT    NULL,
    operand_description TEXT    NULL,
    is_destructive      INTEGER NOT NULL,
    is_native           INTEGER NOT NULL DEFAULT 0,
    record_hash         TEXT    NOT NULL
);

CREATE UNIQUE INDEX IX_operand_record_hash ON operand (record_hash ASC);
CREATE INDEX IX_operand_instruction_key_operand_index ON operand (instruction_key ASC, operand_index ASC);
