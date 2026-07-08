CREATE TABLE tag_consumer
(
    tag_id          INTEGER NOT NULL,
    producer        TEXT    NOT NULL,
    remote_tag      TEXT    NOT NULL,
    remote_instance INTEGER NOT NULL,
    rpi             NUMERIC NOT NULL,
    unicast         INTEGER NOT NULL,
    record_hash     TEXT    NOT NULL,
    FOREIGN KEY (tag_id) REFERENCES tag (tag_id) ON DELETE CASCADE
);
