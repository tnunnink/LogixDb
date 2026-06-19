CREATE TABLE [logix].[aoi_parameter]
(
    [parameter_id]          BIGINT IDENTITY (1, 1) NOT NULL,
    [aoi_id]                BIGINT                 NOT NULL,
    [parameter_name]        NVARCHAR(256)          NOT NULL,
    [parameter_description] NVARCHAR(512)          NULL,
    [data_type]             NVARCHAR(256)          NULL,
    [dimensions]            NVARCHAR(32)           NULL,
    [radix]                 NVARCHAR(32)           NULL,
    [default_value]         NVARCHAR(256)          NULL,
    [external_access]       NVARCHAR(32)           NULL,
    [tag_usage]             NVARCHAR(32)           NULL,
    [tag_type]              NVARCHAR(32)           NULL,
    [tag_alias]             NVARCHAR(128)          NULL,
    [is_visible]            BIT                    NULL,
    [is_required]           BIT                    NULL,
    [is_constant]           BIT                    NULL,
    [record_hash]           NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_aoi_parameter] PRIMARY KEY CLUSTERED ([parameter_id] ASC),
    CONSTRAINT [FK_aoi_parameter_aoi] FOREIGN KEY ([aoi_id]) REFERENCES [logix].[aoi] ([aoi_id]) ON DELETE CASCADE
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_aoi_parameter_aoi_id_record_hash]
    ON [logix].[aoi_parameter] ([aoi_id] ASC, [record_hash] ASC);

CREATE UNIQUE NONCLUSTERED INDEX [IX_aoi_parameter_aoi_id_parameter_name]
    ON [logix].[aoi_parameter] ([aoi_id] ASC, [parameter_name] ASC);

CREATE NONCLUSTERED INDEX [IX_aoi_parameter_parameter_name]
    ON [logix].[aoi_parameter] ([parameter_name] ASC);
