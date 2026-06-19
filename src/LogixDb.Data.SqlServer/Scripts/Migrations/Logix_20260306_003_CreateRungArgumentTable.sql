CREATE TABLE [logix].[rung_argument]
(
    [rung_id]           BIGINT        NOT NULL,
    [instruction_index] SMALLINT      NOT NULL,
    [argument_index]    TINYINT       NOT NULL,
    [argument_type]     NVARCHAR(32)  NOT NULL,
    [argument_text]     NVARCHAR(MAX) NOT NULL,
    [record_hash]       NVARCHAR(64)  NOT NULL,
    CONSTRAINT [FK_rung_argument_rung] FOREIGN KEY ([rung_id]) REFERENCES [logix].[rung] ([rung_id]) ON DELETE CASCADE
);

CREATE UNIQUE CLUSTERED INDEX [IX_rung_argument_rung_id_instruction_index_argument_index]
    ON [logix].[rung_argument] ([rung_id] ASC, [instruction_index] ASC, [argument_index] ASC);
