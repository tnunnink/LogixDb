CREATE TABLE [logix].[rung]
(
    [rung_id]        BIGINT IDENTITY (1, 1) NOT NULL,
    [container_name] NVARCHAR(256)          NOT NULL,
    [routine_name]   NVARCHAR(256)          NOT NULL,
    [rung_number]    INT                    NOT NULL,
    [rung_text]      NVARCHAR(MAX)          NULL,
    [rung_comment]   NVARCHAR(MAX)          NULL,
    [code_hash]      NVARCHAR(64)           NULL,
    [record_hash]    NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_rung] PRIMARY KEY CLUSTERED ([rung_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_rung_record_hash]
    ON [logix].[rung] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_rung_container_name_routine_name_rung_number]
    ON [logix].[rung] ([container_name] ASC, [routine_name] ASC, [rung_number] ASC);

CREATE NONCLUSTERED INDEX [IX_rung_routine_name_rung_number]
    ON [logix].[rung] ([routine_name] ASC, [rung_number] ASC);

CREATE NONCLUSTERED INDEX [IX_rung_code_hash]
    ON [logix].[rung] ([code_hash] ASC);
