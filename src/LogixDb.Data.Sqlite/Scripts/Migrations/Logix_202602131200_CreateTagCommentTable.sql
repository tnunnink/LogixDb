CREATE TABLE tag_member_comment
(
    tag_id      INTEGER NOT NULL,
    member_path TEXT    NOT NULL,
    comment     TEXT    NOT NULL,
    record_hash TEXT    NOT NULL,
    FOREIGN KEY (tag_id) REFERENCES tag (tag_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_tag_member_comment_tag_id_record_hash ON tag_member_comment (tag_id ASC, record_hash ASC);
CREATE UNIQUE INDEX IX_tag_member_comment_tag_id_member_path ON tag_member_comment (tag_id ASC, member_path ASC);
CREATE INDEX IX_tag_member_comment_member_path ON tag_member_comment (member_path ASC);
