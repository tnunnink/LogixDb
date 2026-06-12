namespace LogixDb.Data.Maps;

public class TagValueMap : TableMap<TagValueRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_value";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagValueRecord>> Columns =>
    [
        ColumnMap<TagValueRecord>.For(r => r.VersionId, "version_id"),
        //Both tag hash and member path together will be used to resolve the correct member_id when inserted.
        ColumnMap<TagValueRecord>.For(r => r.TagHash, "tag_hash"),
        ColumnMap<TagValueRecord>.For(r => r.MemberPath, "member_path"),
        ColumnMap<TagValueRecord>.For(r => r.Value, "tag_value")
    ];
}

public record TagValueRecord(int VersionId, string TagHash, string MemberPath, string? Value);