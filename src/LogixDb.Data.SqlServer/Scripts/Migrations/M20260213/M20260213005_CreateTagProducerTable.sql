CREATE TABLE [logix].[tag_producer]
(
    [tag_id]             BIGINT       NOT NULL,
    [produce_count]      INT          NOT NULL,
    [send_event_trigger] BIT          NOT NULL,
    [unicast_permitted]  BIT          NOT NULL,
    [maximum_rpi]        FLOAT        NOT NULL,
    [minimum_rpi]        FLOAT        NOT NULL,
    [default_rpi]        FLOAT        NOT NULL,
    [record_hash]        NVARCHAR(64) NOT NULL,
    CONSTRAINT [FK_tag_producer_tag] FOREIGN KEY ([tag_id]) REFERENCES [logix].[tag] ([tag_id]) ON DELETE CASCADE
);

CREATE CLUSTERED INDEX [IX_tag_producer_tag_id]
    ON [logix].[tag_producer] ([tag_id] ASC);
