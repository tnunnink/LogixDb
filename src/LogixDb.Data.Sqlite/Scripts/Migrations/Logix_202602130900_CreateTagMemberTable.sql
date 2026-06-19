CREATE TABLE tag_member
(
    member_id   INTEGER PRIMARY KEY,
    tag_id      INTEGER NOT NULL,
    member_path TEXT    NOT NULL,
    member_name TEXT    NULL,
    parent_path TEXT    NULL,
    data_type   TEXT    NOT NULL,
    FOREIGN KEY (tag_id) REFERENCES tag (tag_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IX_tag_member_tag_id_member_path ON tag_member (tag_id ASC, member_path ASC);
CREATE INDEX IX_tag_member_member_path ON tag_member (member_path ASC);
CREATE INDEX IX_tag_member_parent_path_tag_id ON tag_member (parent_path ASC, tag_id ASC);
CREATE INDEX IX_tag_member_member_name_tag_id ON tag_member (member_name ASC, tag_id ASC);
CREATE INDEX IX_tag_member_data_type_tag_id ON tag_member (data_type ASC, tag_id ASC);
