CREATE TABLE tag_producer
(
    tag_id             INTEGER NOT NULL,
    produce_count      INTEGER NOT NULL,
    send_event_trigger INTEGER NOT NULL,
    unicast_permitted  INTEGER NOT NULL,
    maximum_rpi        REAL    NOT NULL,
    minimum_rpi        REAL    NOT NULL,
    default_rpi        REAL    NOT NULL,
    record_hash        TEXT    NOT NULL,
    FOREIGN KEY (tag_id) REFERENCES tag (tag_id) ON DELETE CASCADE
);
