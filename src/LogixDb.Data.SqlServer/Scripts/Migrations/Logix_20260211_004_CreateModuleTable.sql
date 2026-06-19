CREATE TABLE [logix].[module]
(
    [module_id]              BIGINT IDENTITY (1, 1) NOT NULL,
    [module_name]            NVARCHAR(256)          NOT NULL,
    [module_description]     NVARCHAR(512)          NULL,
    [catalog_number]         NVARCHAR(64)           NULL,
    [revision]               NVARCHAR(16)           NULL,
    [vendor_id]              INT                    NULL,
    [product_id]             INT                    NULL,
    [product_code]           SMALLINT               NULL,
    [parent_name]            NVARCHAR(256)          NOT NULL,
    [parent_port]            TINYINT                NOT NULL,
    [electronic_keying]      NVARCHAR(32)           NULL,
    [is_inhibited]           BIT                    NULL,
    [is_major_fault_enabled] BIT                    NULL,
    [is_safety_enabled]      BIT                    NULL,
    [config_tag]             NVARCHAR(256)          NULL,
    [ip_address]             NVARCHAR(32)           NULL,
    [slot_number]            TINYINT                NULL,
    [content_hash]           NVARCHAR(64)           NOT NULL,
    [record_hash]            NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_module] PRIMARY KEY CLUSTERED ([module_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_module_record_hash]
    ON [logix].[module] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_module_module_name]
    ON [logix].[module] ([module_name] ASC);

CREATE NONCLUSTERED INDEX [IX_module_parent_name]
    ON [logix].[module] ([parent_name] ASC);
