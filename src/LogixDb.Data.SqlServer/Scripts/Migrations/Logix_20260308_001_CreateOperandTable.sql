CREATE TABLE [logix].[operand]
(
    [operand_id]          BIGINT IDENTITY (1, 1) NOT NULL,
    [instruction_key]     NVARCHAR(128)          NOT NULL,
    [operand_index]       TINYINT                NOT NULL,
    [operand_name]        NVARCHAR(128)          NOT NULL,
    [operand_type]        NVARCHAR(128)          NULL,
    [operand_description] NVARCHAR(2000)         NULL,
    [is_destructive]      BIT                    NOT NULL,
    [is_native]           BIT                    NOT NULL DEFAULT (0),
    [record_hash]         NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_operand] PRIMARY KEY CLUSTERED ([operand_id] ASC)
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_operand_record_hash]
    ON [logix].[operand] ([record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_operand_instruction_key_operand_index]
    ON [logix].[operand] ([instruction_key] ASC, [operand_index] ASC);
