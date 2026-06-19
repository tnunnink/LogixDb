CREATE TABLE target
(
    target_id  INTEGER PRIMARY KEY AUTOINCREMENT,
    target_key TEXT     NOT NULL,
    created_on DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE UNIQUE INDEX IX_target_target_key ON target (target_key ASC);
