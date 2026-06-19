CREATE TABLE [logix].[tag_member]
(
    [member_id]   BIGINT IDENTITY (1, 1) NOT NULL,
    [tag_id]      BIGINT                 NOT NULL,
    [member_path] NVARCHAR(256)          NOT NULL,
    [parent_path] NVARCHAR(128)          NULL,
    [member_name] NVARCHAR(128)          NULL,
    [data_type]   NVARCHAR(128)          NOT NULL,
    CONSTRAINT [PK_tag_member] PRIMARY KEY NONCLUSTERED ([member_id] ASC),
    CONSTRAINT [FK_tag_member_tag] FOREIGN KEY ([tag_id]) REFERENCES [logix].[tag] ([tag_id]) ON DELETE CASCADE
);

CREATE UNIQUE CLUSTERED INDEX [IX_tag_member_tag_id_member_path]
    ON [logix].[tag_member] ([tag_id] ASC, [member_path] ASC);

CREATE NONCLUSTERED INDEX [IX_tag_member_member_path]
    ON [logix].[tag_member] ([member_path] ASC);

CREATE NONCLUSTERED INDEX [IX_tag_member_parent_path_tag_id]
    ON [logix].[tag_member] ([parent_path] ASC, [tag_id] ASC);

CREATE NONCLUSTERED INDEX [IX_tag_member_member_name_tag_id]
    ON [logix].[tag_member] ([member_name] ASC, [tag_id] ASC);

CREATE NONCLUSTERED INDEX [IX_tag_member_data_type_tag_id]
    ON [logix].[tag_member] ([data_type] ASC, [tag_id] ASC);
