CREATE TABLE [logix].[import]
(
    [import_id]     UNIQUEIDENTIFIER NOT NULL,
    [import_status] NVARCHAR(64)     NOT NULL,
    [source_type]   NVARCHAR(64)     NOT NULL,
    [file_type]     NVARCHAR(256)    NOT NULL,
    [file_name]     NVARCHAR(256)    NOT NULL,
    [posted_on]     DATETIME         NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_import] PRIMARY KEY CLUSTERED ([import_id] ASC)
);

CREATE NONCLUSTERED INDEX [IX_import_file_name]
    ON [logix].[import] ([file_name] ASC);

CREATE NONCLUSTERED INDEX [IX_import_posted_on]
    ON [logix].[import] ([posted_on] DESC);

CREATE TABLE [logix].[import_log]
(
    [log_id]        BIGINT IDENTITY (1, 1) NOT NULL,
    [import_id]     UNIQUEIDENTIFIER       NOT NULL,
    [timestamp]     DATETIME               NOT NULL DEFAULT (GETDATE()),
    [log_severity]  NVARCHAR(64)           NOT NULL,
    [log_message]   NVARCHAR(MAX)          NOT NULL,
    [log_exception] NVARCHAR(MAX)          NULL,
    CONSTRAINT [PK_import_log] PRIMARY KEY CLUSTERED ([log_id] ASC),
    CONSTRAINT [FK_import_log_import] FOREIGN KEY ([import_id]) REFERENCES [logix].[import] ([import_id]) ON DELETE CASCADE
);

CREATE NONCLUSTERED INDEX [IX_import_log_import_id]
    ON [logix].[import_log] ([import_id] ASC);
