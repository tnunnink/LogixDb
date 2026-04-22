using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Defines the database table mapping for the <see cref="TagMemberRecord"/> entity.
/// Maps the entity's properties to corresponding database columns, including
/// identifiers, parent-child relationships, and tag details.
/// Provides the structure for schema definitions and facilitates interaction
/// with database records.
/// </summary>
internal class TagMemberMap : TableMap<TagMemberRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag_member";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagMemberRecord>> Columns =>
    [
        ColumnMap<TagMemberRecord>.For(r => r.MemberId, "member_id"),
        ColumnMap<TagMemberRecord>.For(r => r.InstanceId, "instance_id", hashable: false),
        ColumnMap<TagMemberRecord>.For(r => r.TagId, "tag_id"),
        ColumnMap<TagMemberRecord>.For(r => r.ParentId, "parent_id"),
        ColumnMap<TagMemberRecord>.For(r => r.Tag.TagName.LocalPath, "tag_name"),
        ColumnMap<TagMemberRecord>.For(r => r.Tag.TagName.Element, "member_name"),
        ColumnMap<TagMemberRecord>.For(r => r.Tag.GetDataTypeName(), "data_type"),
        ColumnMap<TagMemberRecord>.For(r => r.Tag.Value.GetDataValue(), "tag_value")
    ];
}

/// <summary>
/// Represents a record that defines the mapping of a tag member in the database.
/// Encapsulates the TagId, ParentId, the associated tag, and a unique MemberId.
/// Provides the necessary structure for database interactions and schema definition
/// in conjunction with <see cref="TagMemberMap"/>.
/// </summary>
internal record TagMemberRecord(int InstanceId, Guid TagId, Guid? ParentId, Tag Tag)
{
    public Guid MemberId { get; } = Guid.NewGuid();
}