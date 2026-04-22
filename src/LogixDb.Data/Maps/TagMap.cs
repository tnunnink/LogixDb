using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "tag" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Tag"/> class.
/// </summary>
internal class TagMap : TableMap<TagRecord>
{
    /// <inheritdoc />
    protected override string TableName => "tag";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TagRecord>> Columns =>
    [
        ColumnMap<TagRecord>.For(r => r.TagId, "tag_id", hashable: false),
        ColumnMap<TagRecord>.For(r => r.InstanceId, "instance_id", hashable: false),
        ColumnMap<TagRecord>.For(r => r.ProgramId, "program_id", hashable: false),
        ColumnMap<TagRecord>.For(r => r.Tag.Name, "tag_name", hashable: false),
        ColumnMap<TagRecord>.For(r => r.Tag.DataType, "data_type"),
        ColumnMap<TagRecord>.For(r => r.Tag.Dimensions.ToSqlFormat(), "dimensions"),
        ColumnMap<TagRecord>.For(r => r.Tag.Radix.ToSqlFormat(), "radix"),
        ColumnMap<TagRecord>.For(r => r.Tag.ExternalAccess?.Name, "external_access"),
        ColumnMap<TagRecord>.For(r => r.Tag.OpcUAAccess?.Name ?? Access.None, "opcua_access"),
        ColumnMap<TagRecord>.For(r => r.Tag.Constant ?? false, "is_constant"),
        ColumnMap<TagRecord>.For(r => r.Tag.TagType?.Name ?? TagType.Base, "tag_type"),
        ColumnMap<TagRecord>.For(r => r.Tag.Usage?.Name ?? TagUsage.Normal, "tag_usage"),
        ColumnMap<TagRecord>.For(ComputeHash, "record_hash", hashable: false),
        ColumnMap<TagRecord>.For(r => r.Tag.Hash(), "source_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for a tag entity.
/// This record contains the metadata and configuration for a specific Logix tag,
/// as well as the unique identifier linking it to a specific database target.
/// </summary>
/// <param name="InstanceId">The unique identifier of the instance to which this tag record belongs.</param>
/// <param name="Tag">The Logix tag entity containing its configuration and value.</param>
internal record TagRecord(int InstanceId, Guid? ProgramId, Tag Tag)
{
    public Guid TagId { get; } = Guid.NewGuid();
}
