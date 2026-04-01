namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "tag_alias" table, defining how the
/// fields in the <see cref="TagAliasRecord"/> are associated with the columns in the database table.
/// </summary>
internal class TagAliasMap : TableMap<TagAliasRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_alias";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagAliasRecord>> Columns =>
    [
        ColumnMap<TagAliasRecord>.For(r => r.AliasId, "alias_id", hashable: false),
        ColumnMap<TagAliasRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<TagAliasRecord>.For(r => r.TagId, "tag_id"),
        ColumnMap<TagAliasRecord>.For(r => r.AliasFor, "alias_for")
    ];
}

/// <summary>
/// Represents a record containing alias information associated with a tag.
/// </summary>
/// <param name="SnapshotId">The identifier for the snapshot associated with the alias information.</param>
/// <param name="TagId">The unique identifier for the tag associated with the alias information.</param>
/// <param name="AliasFor">The name or path of the tag that is being aliased.</param>
internal record TagAliasRecord(int SnapshotId, Guid TagId, string AliasFor)
{
    public Guid AliasId { get; } = Guid.NewGuid();
}