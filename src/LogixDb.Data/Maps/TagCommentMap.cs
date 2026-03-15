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
    public override IReadOnlyList<ColumnMap<TagCommentRecord>> Columns =>
    [
        ColumnMap<TagCommentRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<TagCommentRecord>.For(r => r.ProgramName, "program_name"),
        ColumnMap<TagCommentRecord>.For(r => r.TagName, "tag_name"),
        ColumnMap<TagCommentRecord>.For(r => r.TagComment, "tag_comment")
    ];
}

/// <summary>
/// Represents a record for storing user-defined comments associated with specific tags within a snapshot.
/// Each record contains the snapshot identifier, program name, tag name, and the corresponding comment.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot the tag belongs to.</param>
/// <param name="ProgramName">The name of the program associated with the tag.</param>
/// <param name="TagName">The name of the tag being commented on.</param>
/// <param name="TagComment">The user-defined comment for the tag.</param>
public record TagCommentRecord(int SnapshotId, string ProgramName, string TagName, string TagComment);