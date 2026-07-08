CREATE TABLE target_version_map
(
    version_id   INTEGER NOT NULL,
    record_id    INTEGER NOT NULL,
    component_id INTEGER NOT NULL,
    FOREIGN KEY (version_id) REFERENCES target_version (version_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_target_version_map_version_id_component_id_record_id ON target_version_map (version_id ASC, component_id ASC, record_id ASC);
CREATE INDEX IX_target_version_map_record_id_component_id ON target_version_map (record_id ASC, component_id ASC);
