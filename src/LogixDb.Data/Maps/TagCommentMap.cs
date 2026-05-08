using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Defines the mapping configuration for the tag_comment table, which stores user-defined comments
/// associated with tags within a target. Each record uniquely identifies a tag by its hash and name
/// within a specific target.
/// </summary>
internal class TagCommentMap : TableMap<TagCommentRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_comment";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagCommentRecord>> Columns =>
    [
        ColumnMap<TagCommentRecord>.For(r => r.MemberId, "member_id"),
        ColumnMap<TagCommentRecord>.For(r => r.TagName, "tag_name"),
        ColumnMap<TagCommentRecord>.For(r => r.TagComment, "tag_comment"),
        ColumnMap<TagCommentRecord>.For(r => r.Hash(["MemberId"]), "record_hash")
    ];
}

internal record TagCommentRecord(string? MemberId, string TagName, string TagComment);