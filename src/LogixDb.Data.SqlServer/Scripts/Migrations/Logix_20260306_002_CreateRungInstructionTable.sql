CREATE TABLE [logix].[rung_instruction]
(
    [rung_id]           BIGINT        NOT NULL,
    [instruction_index] SMALLINT      NOT NULL,
    [instruction_text]  NVARCHAR(MAX) NOT NULL,
    [instruction_key]   NVARCHAR(128) NOT NULL,
    [is_conditional]    BIT           NOT NULL,
    [is_native]         BIT           NOT NULL,
    [record_hash]       NVARCHAR(64)  NOT NULL,
    CONSTRAINT [FK_rung_instruction_rung] FOREIGN KEY ([rung_id]) REFERENCES [logix].[rung] ([rung_id]) ON DELETE CASCADE
);

CREATE UNIQUE CLUSTERED INDEX [IX_rung_instruction_rung_id_instruction_index]
    ON [logix].[rung_instruction] ([rung_id] ASC, [instruction_index] ASC);

CREATE NONCLUSTERED INDEX [IX_rung_instruction_instruction_key]
    ON [logix].[rung_instruction] ([instruction_key] ASC);

CREATE NONCLUSTERED INDEX [IX_rung_instruction_record_hash]
    ON [logix].[rung_instruction] ([record_hash] ASC);
