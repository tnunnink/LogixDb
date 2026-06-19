CREATE TABLE program
(
    program_id          INTEGER PRIMARY KEY,
    program_name        TEXT    NOT NULL,
    task_name           TEXT    NULL,
    folder_name         TEXT    NULL,
    program_description TEXT    NULL,
    program_type        TEXT    NULL,
    main_routine        TEXT    NULL,
    fault_routine       TEXT    NULL,
    is_disabled         INTEGER NULL,
    is_folder           INTEGER NULL,
    has_test_edits      INTEGER NULL,
    record_hash         TEXT    NOT NULL
);

CREATE UNIQUE INDEX IX_program_record_hash ON program (record_hash ASC);
CREATE INDEX IX_program_program_name ON program (program_name ASC);
CREATE INDEX IX_program_folder_name ON program (folder_name ASC);
CREATE INDEX IX_program_task_name ON program (task_name ASC);
