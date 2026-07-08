CREATE TABLE [logix].[tag_value]
(
    [version_id] INT           NULL,
    [member_id]  BIGINT        NOT NULL,
    [tag_value]  NVARCHAR(256) NOT NULL,
    CONSTRAINT [FK_tag_value_target_version] FOREIGN KEY ([version_id]) REFERENCES [logix].[target_version] ([version_id]) ON DELETE SET NULL,
    CONSTRAINT [FK_tag_value_tag_member] FOREIGN KEY ([member_id]) REFERENCES [logix].[tag_member] ([member_id]) ON DELETE CASCADE
);

CREATE CLUSTERED INDEX [IX_tag_value_version_id_member_id]
    ON [logix].[tag_value] ([version_id] ASC, [member_id] ASC);
