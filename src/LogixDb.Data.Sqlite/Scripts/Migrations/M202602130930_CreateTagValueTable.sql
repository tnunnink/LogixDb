CREATE TABLE tag_value
(
    version_id INTEGER NULL,
    member_id  INTEGER NOT NULL,
    tag_value  TEXT    NOT NULL,
    FOREIGN KEY (version_id) REFERENCES target_version (version_id) ON DELETE SET NULL,
    FOREIGN KEY (member_id) REFERENCES tag_member (member_id) ON DELETE CASCADE
);

CREATE INDEX IX_tag_value_version_id_member_id ON tag_value (version_id ASC, member_id ASC);
