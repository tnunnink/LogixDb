CREATE TABLE aoi_parameter
(
    parameter_id          INTEGER PRIMARY KEY,
    aoi_id                INTEGER NOT NULL,
    parameter_name        TEXT    NOT NULL,
    parameter_description TEXT    NULL,
    data_type             TEXT    NULL,
    dimensions            TEXT    NULL,
    radix                 TEXT    NULL,
    default_value         TEXT    NULL,
    external_access       TEXT    NULL,
    tag_usage             TEXT    NULL,
    tag_type              TEXT    NULL,
    tag_alias             TEXT    NULL,
    is_visible            INTEGER NULL,
    is_required           INTEGER NULL,
    is_constant           INTEGER NULL,
    record_hash           TEXT    NOT NULL,
    FOREIGN KEY (aoi_id) REFERENCES aoi (aoi_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_aoi_parameter_aoi_id_record_hash ON aoi_parameter (aoi_id ASC, record_hash ASC);
CREATE UNIQUE INDEX IX_aoi_parameter_aoi_id_parameter_name ON aoi_parameter (aoi_id ASC, parameter_name ASC);
CREATE INDEX IX_aoi_parameter_parameter_name ON aoi_parameter (parameter_name ASC);
