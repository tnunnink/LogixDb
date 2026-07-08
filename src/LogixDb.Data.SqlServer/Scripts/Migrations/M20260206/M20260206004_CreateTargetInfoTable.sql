CREATE TABLE [logix].[target_info]
(
    [property_id]    UNIQUEIDENTIFIER NOT NULL,
    [version_id]     INT              NOT NULL,
    [property_name]  NVARCHAR(255)    NOT NULL,
    [property_value] NVARCHAR(MAX)    NULL,
    CONSTRAINT [PK_target_info] PRIMARY KEY CLUSTERED ([property_id] ASC),
    CONSTRAINT [FK_target_info_target_version] FOREIGN KEY ([version_id]) REFERENCES [logix].[target_version] ([version_id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_target_info_version_id_property_name]
    ON [logix].[target_info] ([version_id] ASC, [property_name] ASC);
