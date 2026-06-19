CREATE TABLE [logix].[controller]
(
    [controller_id]               BIGINT IDENTITY (1, 1) NOT NULL,
    [controller_name]             NVARCHAR(256)          NOT NULL,
    [controller_description]      NVARCHAR(512)          NULL,
    [catalog_number]              NVARCHAR(256)          NULL,
    [revision]                    NVARCHAR(32)           NULL,
    [project_creation_date]       DATETIME               NULL,
    [communication_path]          NVARCHAR(256)          NULL,
    [sfc_execution_control]       NVARCHAR(32)           NULL,
    [sfc_restart_position]        NVARCHAR(32)           NULL,
    [sfc_last_scan]               NVARCHAR(32)           NULL,
    [project_serial_number]       NVARCHAR(32)           NULL,
    [match_project_to_controller] BIT                    NULL,
    [inhibit_firmware_updates]    BIT                    NULL,
    [allow_rfi_from_producer]     BIT                    NULL,
    [pass_through_option]         NVARCHAR(32)           NULL,
    [download_documentation]      BIT                    NULL,
    [download_properties]         BIT                    NULL,
    [ethernet_ip_mode]            NVARCHAR(32)           NULL,
    [redundancy_enabled]          BIT                    NULL,
    [keep_test_edits_on_switch]   BIT                    NULL,
    [io_memory_pad_percent]       REAL                   NULL,
    [data_table_pad_percent]      REAL                   NULL,
    [safety_signature]            NVARCHAR(32)           NULL,
    [safety_lock_password]        NVARCHAR(32)           NULL,
    [safety_unlock_password]      NVARCHAR(32)           NULL,
    [configure_safety_io_always]  BIT                    NULL,
    [signature_run_mode_protect]  BIT                    NULL,
    [security_code]               INT                    NULL,
    [security_authority_id]       NVARCHAR(MAX)          NULL,
    [security_authority_uri]      NVARCHAR(MAX)          NULL,
    [permission_set]              NVARCHAR(MAX)          NULL,
    [changed_to_detect]           NVARCHAR(MAX)          NULL,
    [trusted_slots]               NVARCHAR(64)           NULL,
    [record_hash]                 NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_controller] PRIMARY KEY CLUSTERED ([controller_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_controller_record_hash]
    ON [logix].[controller] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_controller_controller_name]
    ON [logix].[controller] ([controller_name] ASC);
