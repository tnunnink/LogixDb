using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "aoi_parameter" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Parameter"/> class.
/// </summary>
internal class AoiParameterMap : TableMap<AoiParameterRecord>
{
    /// <inheritdoc />
    protected override string TableName => "aoi_parameter";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<AoiParameterRecord>> Columns =>
    [
        ColumnMap<AoiParameterRecord>.For(r => r.ParameterId, "parameter_id"),
        ColumnMap<AoiParameterRecord>.For(r => r.InstanceId, "instance_id", hashable: false),
        ColumnMap<AoiParameterRecord>.For(r => r.AoiId, "aoi_id"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.Name, "parameter_name", hashable: false),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.Description, "parameter_description"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.DataType, "data_type"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.Dimension.ToSqlFormat(), "dimensions"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.Radix.ToSqlFormat(), "radix"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.Default?.GetDataValue(), "default_value"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.ExternalAccess?.Name, "external_access"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.Usage.Name, "tag_usage"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.TagType?.Name, "tag_type"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.AliasFor?.LocalPath, "tag_alias"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.Visible, "is_visible"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.Required, "is_required"),
        ColumnMap<AoiParameterRecord>.For(r => r.Parameter.Constant, "is_constant"),
        ColumnMap<AoiParameterRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for an AOI parameter entity.
/// This record contains the metadata and configuration for a specific AOI parameter,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
internal record AoiParameterRecord(int InstanceId, Guid? AoiId, Parameter Parameter)
{
    public Guid ParameterId { get; } = Guid.NewGuid();
}