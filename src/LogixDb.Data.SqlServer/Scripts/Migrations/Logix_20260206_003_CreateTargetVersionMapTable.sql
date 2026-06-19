CREATE TABLE [logix].[target_version_map]
(
    [version_id]   INT     NOT NULL,
    [record_id]    BIGINT  NOT NULL,
    [component_id] TINYINT NOT NULL,
    CONSTRAINT [FK_target_version_map_target_version] FOREIGN KEY ([version_id]) REFERENCES [logix].[target_version] ([version_id]) ON DELETE CASCADE
);

CREATE UNIQUE CLUSTERED INDEX [IX_target_version_map_version_id_component_id_record_id]
    ON [logix].[target_version_map] ([version_id] ASC, [component_id] ASC, [record_id] ASC);

CREATE NONCLUSTERED INDEX [IX_target_version_map_record_id_component_id]
    ON [logix].[target_version_map] ([record_id] ASC, [component_id] ASC);
