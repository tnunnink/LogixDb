CREATE TABLE target_info
(
    property_id    UNIQUEIDENTIFIER PRIMARY KEY,
    version_id     INTEGER NOT NULL,
    property_name  TEXT    NOT NULL,
    property_value TEXT    NULL,
    FOREIGN KEY (version_id) REFERENCES target_version (version_id) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE UNIQUE INDEX IX_target_info_version_id_property_name ON target_info (version_id ASC, property_name ASC);
