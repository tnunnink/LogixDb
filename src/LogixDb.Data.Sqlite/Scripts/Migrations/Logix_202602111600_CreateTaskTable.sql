CREATE TABLE task
(
    task_id          INTEGER PRIMARY KEY,
    task_name        TEXT    NOT NULL,
    task_description TEXT    NULL,
    task_type        TEXT    NULL,
    priority         INTEGER NULL,
    scan_rate        REAL    NULL,
    watchdog         REAL    NULL,
    is_inhibited     INTEGER NULL,
    disable_outputs  INTEGER NULL,
    event_trigger    TEXT    NULL,
    event_tag        TEXT    NULL,
    enable_timeout   INTEGER NULL,
    record_hash      TEXT    NOT NULL
);

CREATE UNIQUE INDEX IX_task_record_hash ON task (record_hash ASC);
CREATE INDEX IX_task_task_name ON task (task_name ASC);
