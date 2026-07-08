CREATE TABLE [logix].[tag_consumer]
(
    [tag_id]          BIGINT       NOT NULL,
    [producer]        NVARCHAR(255) NOT NULL,
    [remote_tag]      NVARCHAR(255) NOT NULL,
    [remote_instance] INT          NOT NULL,
    [rpi]             FLOAT        NOT NULL,
    [unicast]         BIT          NOT NULL,
    [record_hash]     NVARCHAR(64) NOT NULL,
    CONSTRAINT [FK_tag_consumer_tag] FOREIGN KEY ([tag_id]) REFERENCES [logix].[tag] ([tag_id]) ON DELETE CASCADE
);

CREATE CLUSTERED INDEX [IX_tag_consumer_tag_id]
    ON [logix].[tag_consumer]([tag_id] ASC);
