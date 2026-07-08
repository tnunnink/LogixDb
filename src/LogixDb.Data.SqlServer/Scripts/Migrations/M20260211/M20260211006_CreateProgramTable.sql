CREATE TABLE [logix].[program]
(
    [program_id]          BIGINT IDENTITY (1, 1) NOT NULL,
    [program_name]        NVARCHAR(256)          NOT NULL,
    [task_name]           NVARCHAR(256)          NULL,
    [folder_name]         NVARCHAR(256)          NULL,
    [program_description] NVARCHAR(512)          NULL,
    [program_type]        NVARCHAR(32)           NULL,
    [main_routine]        NVARCHAR(64)           NULL,
    [fault_routine]       NVARCHAR(64)           NULL,
    [is_disabled]         BIT                    NULL,
    [is_folder]           BIT                    NULL,
    [has_test_edits]      BIT                    NULL,
    [record_hash]         NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_program] PRIMARY KEY CLUSTERED ([program_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_program_record_hash]
    ON [logix].[program] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_program_program_name]
    ON [logix].[program] ([program_name] ASC);

CREATE NONCLUSTERED INDEX [IX_program_folder_name]
    ON [logix].[program] ([folder_name] ASC);

CREATE NONCLUSTERED INDEX [IX_program_task_name]
    ON [logix].[program] ([task_name] ASC);
