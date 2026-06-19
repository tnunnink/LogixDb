CREATE TABLE [logix].[tag_member_comment]
(
    [tag_id]      BIGINT        NOT NULL,
    [member_path] NVARCHAR(256) NOT NULL,
    [comment]     NVARCHAR(MAX) NOT NULL,
    [record_hash] NVARCHAR(64)  NOT NULL,
    CONSTRAINT [FK_tag_member_comment_tag] FOREIGN KEY ([tag_id]) REFERENCES [logix].[tag] ([tag_id]) ON DELETE CASCADE
);

CREATE UNIQUE CLUSTERED INDEX [IX_tag_member_comment_tag_id_member_path]
    ON [logix].[tag_member_comment] ([tag_id] ASC, [member_path] ASC);

CREATE UNIQUE NONCLUSTERED INDEX [IX_tag_member_comment_tag_id_record_hash]
    ON [logix].[tag_member_comment] ([tag_id] ASC, [record_hash] ASC);

CREATE NONCLUSTERED INDEX [IX_tag_member_comment_member_path]
    ON [logix].[tag_member_comment] ([member_path] ASC);
