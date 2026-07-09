CREATE TABLE [logix].[rung_reference]
(
    [rung_id]           BIGINT        NOT NULL,
    [instruction_index] SMALLINT      NOT NULL,
    [argument_index]    TINYINT       NOT NULL,
    [base_reference]    NVARCHAR(256) NOT NULL,
    [member_reference]  NVARCHAR(256) NOT NULL,
    CONSTRAINT [FK_rung_reference_rung] FOREIGN KEY ([rung_id]) REFERENCES [logix].[rung] ([rung_id]) ON DELETE CASCADE
);

CREATE CLUSTERED INDEX [IX_rung_reference_rung_id_instruction_index_argument_index]
    ON [logix].[rung_reference] ([rung_id] ASC, [instruction_index] ASC, [argument_index] ASC);

CREATE NONCLUSTERED INDEX [IX_rung_reference_base_reference]
    ON [logix].[rung_reference] ([base_reference] ASC);
