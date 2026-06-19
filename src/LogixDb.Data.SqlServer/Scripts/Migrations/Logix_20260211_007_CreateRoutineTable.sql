CREATE TABLE [logix].[routine]
(
    [routine_id]          BIGINT IDENTITY (1, 1) NOT NULL,
    [container_name]      NVARCHAR(256)          NOT NULL,
    [routine_name]        NVARCHAR(256)          NOT NULL,
    [routine_description] NVARCHAR(512)          NULL,
    [routine_type]        NVARCHAR(32)           NULL,
    [is_definition]       BIT                    NOT NULL,
    [record_hash]         NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_routine] PRIMARY KEY CLUSTERED ([routine_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_routine_record_hash]
    ON [logix].[routine] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_routine_container_name_routine_name]
    ON [logix].[routine] ([container_name] ASC, [routine_name] ASC);

CREATE NONCLUSTERED INDEX [IX_routine_routine_name]
    ON [logix].[routine] ([routine_name] ASC);
