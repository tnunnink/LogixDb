namespace LogixDb.Data.Maps;

internal class TagValueMap : TableMap<TagValueRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_value";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagValueRecord>> Columns =>
    [
        ColumnMap<TagValueRecord>.For(r => r.VersionId, "version_id"),
        ColumnMap<TagValueRecord>.For(r => r.MemberId, "member_id"),
        ColumnMap<TagValueRecord>.For(r => r.Value, "tag_value")
    ];
}

internal record TagValueRecord(int VersionId, string? MemberId, string? Value);