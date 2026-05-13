namespace LogixDb.Data.Maps;

public class TagValueMap : TableMap<TagValueRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_value";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagValueRecord>> Columns =>
    [
        ColumnMap<TagValueRecord>.For(r => r.VersionId, "version_id"),
        ColumnMap<TagValueRecord>.For(r => r.TagHash, "tag_hash"),
        ColumnMap<TagValueRecord>.For(r => r.TagName, "tag_name"),
        ColumnMap<TagValueRecord>.For(r => r.Value, "tag_value")
    ];
}

public record TagValueRecord(int VersionId, string TagHash, string TagName, string? Value);