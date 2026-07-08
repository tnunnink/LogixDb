CREATE TABLE [logix].[task]
(
    [task_id]          BIGINT IDENTITY (1, 1) NOT NULL,
    [task_name]        NVARCHAR(256)          NOT NULL,
    [task_description] NVARCHAR(512)          NULL,
    [task_type]        NVARCHAR(32)           NULL,
    [priority]         TINYINT                NULL,
    [scan_rate]        REAL                   NULL,
    [watchdog]         REAL                   NULL,
    [is_inhibited]     BIT                    NULL,
    [disable_outputs]  BIT                    NULL,
    [event_trigger]    NVARCHAR(32)           NULL,
    [event_tag]        NVARCHAR(128)          NULL,
    [enable_timeout]   BIT                    NULL,
    [record_hash]      NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_task] PRIMARY KEY CLUSTERED ([task_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_task_record_hash]
    ON [logix].[task] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_task_task_name]
    ON [logix].[task] ([task_name] ASC);
