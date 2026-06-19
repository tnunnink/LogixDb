CREATE TABLE target_component
(
    component_id   INTEGER PRIMARY KEY AUTOINCREMENT,
    component_name TEXT NOT NULL
);

CREATE UNIQUE INDEX IX_target_component_component_name ON target_component (component_name ASC);
