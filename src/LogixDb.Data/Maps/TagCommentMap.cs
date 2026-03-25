namespace LogixDb.Data.Maps;

/// <summary>
/// Defines the mapping configuration for the tag_comment table, which stores user-defined comments
/// associated with tags within a snapshot. Each record uniquely identifies a tag by its hash and name
/// within a specific snapshot.
/// </summary>
public class TagCommentMap : TableMap<TagCommentRecord>
{
    /// <inheritdoc />
    public override string TableName => "tag_comment";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagCommentRecord>> Columns =>
    [
        ColumnMap<TagCommentRecord>.For(r => r.CommentId, "comment_id", hashable: false),
        ColumnMap<TagCommentRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<TagCommentRecord>.For(r => r.TagId, "tag_id"),
        ColumnMap<TagCommentRecord>.For(r => r.TagName, "tag_name"),
        ColumnMap<TagCommentRecord>.For(r => r.TagComment, "tag_comment")
    ];
}

/// <summary>
/// Represents a record containing information about a tag's comments within a specific snapshot.
/// Each record uniquely identifies a comment by associating a unique tag identifier, snapshot identifier,
/// tag name, and its comment text.
/// </summary>
public record TagCommentRecord(int SnapshotId, Guid TagId, string TagName, string TagComment)
{
    public Guid CommentId { get; } = Guid.NewGuid();
}