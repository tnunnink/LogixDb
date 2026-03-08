using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "tag" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Tag"/> class.
/// </summary>
public class TagMap : TableMap<TagRecord>
{
    /// <inheritdoc />
    public override string TableName => "tag";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<TagRecord>> Columns =>
    [
        ColumnMap<TagRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<TagRecord>.For(r => r.Tag.Scope.Container, "container_name"),
        ColumnMap<TagRecord>.For(r => r.Tag.TagName.LocalPath, "tag_name"),
        ColumnMap<TagRecord>.For(r => r.Tag.TagName.Base, "base_name"),
        ColumnMap<TagRecord>.For(r => r.Tag.Parent?.TagName.LocalPath, "parent_name"),
        ColumnMap<TagRecord>.For(r => r.Tag.TagName.Element, "member_name"),
        ColumnMap<TagRecord>.For(r => r.Tag.Value.IsAtomic() ? r.Tag.Value.ToString() : null, "tag_value"),
        ColumnMap<TagRecord>.For(
            r => r.Tag.Dimensions.IsEmpty ? r.Tag.DataType : $"{r.Tag.DataType}{r.Tag.Dimensions.ToIndex()}",
            "data_type"),
        ColumnMap<TagRecord>.For(r => r.Tag.Description, "description"),
        ColumnMap<TagRecord>.For(r => r.Tag.ExternalAccess?.Name, "external_access"),
        ColumnMap<TagRecord>.For(r => r.Tag.Constant, "constant"),
        ColumnMap<TagRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for a tag entity.
/// This record contains the metadata and configuration for a specific Logix tag,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this tag record belongs.</param>
/// <param name="Tag">The Logix tag entity containing its configuration and value.</param>
public record TagRecord(int SnapshotId, Tag Tag);