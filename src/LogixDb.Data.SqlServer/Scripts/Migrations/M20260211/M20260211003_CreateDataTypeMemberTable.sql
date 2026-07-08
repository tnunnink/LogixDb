CREATE TABLE [logix].[data_type_member]
(
    [member_id]          BIGINT IDENTITY (1, 1) NOT NULL,
    [type_id]            BIGINT                 NOT NULL,
    [member_name]        NVARCHAR(256)          NOT NULL,
    [member_description] NVARCHAR(512)          NULL,
    [member_index]       INT                    NOT NULL,
    [data_type]          NVARCHAR(256)          NULL,
    [dimensions]         NVARCHAR(32)           NULL,
    [radix]              NVARCHAR(32)           NULL,
    [external_access]    NVARCHAR(32)           NULL,
    [is_hidden]          BIT                    NULL,
    [target_name]        NVARCHAR(64)           NULL,
    [bit_number]         TINYINT                NULL,
    [record_hash]        NVARCHAR(64)           NOT NULL,
    CONSTRAINT [PK_data_type_member] PRIMARY KEY NONCLUSTERED ([member_id] ASC),
    CONSTRAINT [FK_data_type_member_data_type] FOREIGN KEY ([type_id]) REFERENCES [logix].[data_type] ([type_id]) ON DELETE CASCADE
);

CREATE UNIQUE CLUSTERED INDEX [IX_data_type_member_type_id_member_name]
    ON [logix].[data_type_member] ([type_id] ASC, [member_name] ASC);

CREATE UNIQUE NONCLUSTERED INDEX [IX_data_type_member_type_id_record_hash]
    ON [logix].[data_type_member] ([type_id] ASC, [record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_data_type_member_member_name]
    ON [logix].[data_type_member] ([member_name] ASC);
