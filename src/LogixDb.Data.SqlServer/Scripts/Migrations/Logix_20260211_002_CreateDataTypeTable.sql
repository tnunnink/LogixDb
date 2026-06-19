CREATE TABLE [logix].[data_type]
(
    [type_id]          BIGINT IDENTITY (1, 1) NOT NULL,
    [type_name]        NVARCHAR(256)          NOT NULL,
    [type_description] NVARCHAR(512)          NULL,
    [type_class]       NVARCHAR(32)           NULL,
    [type_family]      NVARCHAR(32)           NULL,
    [content_hash]     NVARCHAR(64)           NOT NULL,
    [record_hash]      NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_data_type] PRIMARY KEY CLUSTERED ([type_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_data_type_record_hash]
    ON [logix].[data_type] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_data_type_type_name]
    ON [logix].[data_type] ([type_name] ASC);
