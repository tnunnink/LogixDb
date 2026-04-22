using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for LocalTag objects to the "aoi_parameter" database table.
/// This class defines how specific properties of LocalTag elements are mapped to corresponding table columns.
/// </summary>
internal class AoiLocalTagMap : TableMap<AoiLocalTagRecord>
{
    /// <inheritdoc />
    protected override string TableName => "aoi_parameter";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<AoiLocalTagRecord>> Columns =>
    [
        ColumnMap<AoiLocalTagRecord>.For(r => r.ParameterId, "parameter_id"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.InstanceId, "instance_id", hashable: false),
        ColumnMap<AoiLocalTagRecord>.For(r => r.AoiId, "aoi_id"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Name, "parameter_name", hashable: false),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Description, "parameter_description"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.DataType, "data_type"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Dimensions.ToSqlFormat(), "dimensions"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Radix.ToSqlFormat(), "radix"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Value.GetDataValue(), "default_value"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.ExternalAccess?.Name, "external_access"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Usage?.Name, "tag_usage"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.TagType?.Name, "tag_type"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.AliasFor?.LocalPath, "tag_alias"),
        ColumnMap<AoiLocalTagRecord>.For(_ => false, "is_visible"),
        ColumnMap<AoiLocalTagRecord>.For(_ => false, "is_required"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Constant, "is_constant"),
        ColumnMap<AoiLocalTagRecord>.For(ComputeHash, "record_hash", false)
    ];
}

/// <summary>
/// Represents a record for an AOI (Add-On Instruction) local tag that is mapped to the "aoi_parameter" database table.
/// This record contains relevant information for uniquely identifying and associating an AOI tag with its attributes.
/// </summary>
/// <remarks>
/// The <see cref="AoiLocalTagRecord"/> serves as a structural definition that encapsulates properties such as the instance identifier,
/// the name of the associated AOI, and the tag details.
/// This record is used in mapping operations for transferring data between the program and the database storage.
/// </remarks>
internal record AoiLocalTagRecord(int InstanceId, Guid? AoiId, LocalTag Tag)
{
    public Guid ParameterId { get; } = Guid.NewGuid();
}