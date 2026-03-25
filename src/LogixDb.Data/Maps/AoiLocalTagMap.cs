using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for LocalTag objects to the "aoi_parameter" database table.
/// This class defines how specific properties of LocalTag elements are mapped to corresponding table columns.
/// </summary>
/// <remarks>
/// This is my way of combining the local tags and parameters for an AOI into a single table in the database.
/// They are essentially all the same columns, except that the local tag doesn't have required and visible properties.
/// I'd prefer not to make separate tables and not to tread these as "tag" instances but as member definitions.
/// </remarks>
internal class AoiLocalTagMap : TableMap<AoiLocalTagRecord>
{
    /// <inheritdoc />
    public override string TableName => "aoi_parameter";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<AoiLocalTagRecord>> Columns =>
    [
        ColumnMap<AoiLocalTagRecord>.For(r => r.ParameterId, "parameter_id", hashable: false),
        ColumnMap<AoiLocalTagRecord>.For(r => r.SnapshotId, "snapshot_id", false),
        ColumnMap<AoiLocalTagRecord>.For(r => r.AoiName, "aoi_name"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Name, "parameter_name"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.GetDataTypeName(), "data_type"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Value.GetDataValue(), "default_value"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Description, "parameter_description"),
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
/// The <see cref="AoiLocalTagRecord"/> serves as a structural definition that encapsulates properties such as the snapshot identifier,
/// the name of the associated AOI, and the tag details.
/// This record is used in mapping operations for transferring data between the program and the database storage.
/// </remarks>
public record AoiLocalTagRecord(int SnapshotId, string AoiName, LocalTag Tag)
{
    public Guid ParameterId { get; } = Guid.NewGuid();
}