namespace LogixDb.Data.Maps;

internal class TagCommentMap : TableMap<TagCommentRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_comment";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagCommentRecord>> Columns =>
    [
        ColumnMap<TagCommentRecord>.For(r => r.TagHash, "tag_hash"),
        ColumnMap<TagCommentRecord>.For(r => r.TagName, "member_tag"),
        ColumnMap<TagCommentRecord>.For(r => r.TagName, "tag_name"),
        ColumnMap<TagCommentRecord>.For(r => r.TagComment, "tag_comment"),
        ColumnMap<TagCommentRecord>.For(ComputeHash, "record_hash")
    ];
}

internal record TagCommentRecord(string? TagHash, string TagName, string TagComment);