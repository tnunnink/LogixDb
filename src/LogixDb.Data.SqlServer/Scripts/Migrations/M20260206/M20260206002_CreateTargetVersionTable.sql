CREATE TABLE [logix].[target_version]
(
    [version_id]        INT IDENTITY (1, 1) NOT NULL,
    [target_id]         INT                 NOT NULL,
    [version_number]    INT                 NOT NULL,
    [target_type]       NVARCHAR(128)       NOT NULL,
    [target_name]       NVARCHAR(128)       NULL,
    [target_count]      INT                 NULL,
    [schema_revision]   NVARCHAR(16)        NULL,
    [software_revision] NVARCHAR(16)        NULL,
    [is_partial]        BIT                 NOT NULL,
    [import_date]       DATETIME            NOT NULL,
    [import_user]       NVARCHAR(64)        NOT NULL,
    [import_machine]    NVARCHAR(64)        NOT NULL,
    [export_date]       DATETIME            NULL,
    [export_options]    NVARCHAR(256)       NULL,
    [source_hash]       NVARCHAR(64)        NOT NULL,
    [source_data]       VARBINARY(MAX)      NOT NULL,
    CONSTRAINT [PK_target_version] PRIMARY KEY CLUSTERED ([version_id] ASC),
    CONSTRAINT [FK_target_version_target] FOREIGN KEY ([target_id]) REFERENCES [logix].[target] ([target_id]) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_target_version_target_id_version_number]
    ON [logix].[target_version] ([target_id] ASC, [version_number] ASC);

CREATE NONCLUSTERED INDEX [IX_target_version_target_id_import_date]
    ON [logix].[target_version] ([target_id] ASC, [import_date] DESC);

CREATE NONCLUSTERED INDEX [IX_target_version_target_name_version_number]
    ON [logix].[target_version] ([target_name] ASC, [version_number] ASC);
