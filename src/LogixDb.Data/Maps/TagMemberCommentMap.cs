namespace LogixDb.Data.Maps;

public class TagMemberCommentMap : TableMap<TagCommentRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_member_comment";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagCommentRecord>> Columns =>
    [
        ColumnMap<TagCommentRecord>.For(r => r.TagHash, "tag_hash", hashable: false),
        ColumnMap<TagCommentRecord>.For(r => r.MemberPath, "member_path"),
        ColumnMap<TagCommentRecord>.For(r => r.Comment, "comment"),
        ColumnMap<TagCommentRecord>.RecordHash(this)
    ];
}

public record TagCommentRecord(string? TagHash, string MemberPath, string Comment);