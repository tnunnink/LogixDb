IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'logix')
BEGIN
    EXEC('CREATE SCHEMA [logix]')
END

CREATE TABLE [logix].[target]
(
    [target_id]  INT IDENTITY (1, 1) NOT NULL,
    [target_key] NVARCHAR(128)       NOT NULL,
    [created_on] DATETIME            NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_target] PRIMARY KEY CLUSTERED ([target_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_target_target_key]
    ON [logix].[target]([target_key] ASC);
