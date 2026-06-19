CREATE TABLE [logix].[aoi]
(
    [aoi_id]                  BIGINT IDENTITY (1, 1) NOT NULL,
    [aoi_name]                NVARCHAR(256)          NOT NULL,
    [aoi_description]         NVARCHAR(512)          NULL,
    [aoi_revision]            NVARCHAR(16)           NULL,
    [aoi_revision_extension]  NVARCHAR(64)           NULL,
    [aoi_revision_note]       NVARCHAR(512)          NULL,
    [aoi_vendor]              NVARCHAR(64)           NULL,
    [aoi_help_text]           NVARCHAR(MAX)          NULL,
    [created_date]            DATETIME               NULL,
    [created_by]              NVARCHAR(64)           NULL,
    [edited_date]             DATETIME               NULL,
    [edited_by]               NVARCHAR(64)           NULL,
    [software_revision]       NVARCHAR(16)           NULL,
    [execute_pre_scan]        BIT                    NULL,
    [execute_post_scan]       BIT                    NULL,
    [execute_enable_in_false] BIT                    NULL,
    [is_encrypted]            BIT                    NULL,
    [signature_id]            NVARCHAR(32)           NULL,
    [signature_timestamp]     DATETIME               NULL,
    [component_class]         NVARCHAR(32)           NULL,
    [content_hash]            NVARCHAR(64)           NOT NULL,
    [record_hash]             NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_aoi] PRIMARY KEY CLUSTERED ([aoi_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_aoi_record_hash]
    ON [logix].[aoi] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_aoi_aoi_name]
    ON [logix].[aoi] ([aoi_name] ASC);
