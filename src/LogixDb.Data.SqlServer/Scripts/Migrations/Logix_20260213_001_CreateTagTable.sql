CREATE TABLE [logix].[tag]
(
    [tag_id]          BIGINT IDENTITY (1, 1) NOT NULL,
    [program_name]    NVARCHAR(256)          NULL,
    [tag_name]        NVARCHAR(256)          NOT NULL,
    [tag_description] NVARCHAR(512)          NULL,
    [data_type]       NVARCHAR(128)          NULL,
    [dimensions]      NVARCHAR(32)           NULL,
    [radix]           NVARCHAR(32)           NULL,
    [external_access] NVARCHAR(32)           NULL,
    [opcua_access]    NVARCHAR(32)           NULL,
    [is_constant]     BIT                    NULL,
    [tag_usage]       NVARCHAR(32)           NULL,
    [tag_type]        NVARCHAR(32)           NULL,
    [alias_for]       NVARCHAR(256)          NULL,
    [content_hash]    NVARCHAR(64)           NOT NULL,
    [record_hash]     NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_tag] PRIMARY KEY CLUSTERED ([tag_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_tag_record_hash]
    ON [logix].[tag] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_tag_program_name_tag_name]
    ON [logix].[tag] ([program_name] ASC, [tag_name] ASC);

CREATE NONCLUSTERED INDEX [IX_tag_tag_name]
    ON [logix].[tag] ([tag_name] ASC);

CREATE NONCLUSTERED INDEX [IX_tag_data_type]
    ON [logix].[tag] ([data_type] ASC);
