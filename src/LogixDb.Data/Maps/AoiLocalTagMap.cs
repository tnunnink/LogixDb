using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for LocalTag objects to the "aoi_parameter" database table.
/// This class defines how specific properties of LocalTag elements are mapped to corresponding table columns.
/// </summary>
public class AoiLocalTagMap : TableMap<LocalTag>
{
    /// <inheritdoc />
    protected override string TableName => "aoi_parameter";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<LocalTag>> Columns =>
    [
        ColumnMap<LocalTag>.For(r => r.Metadata.Get<string>("aoi_hash"), "aoi_hash"),
        ColumnMap<LocalTag>.For(r => r.Name, "parameter_name"),
        ColumnMap<LocalTag>.For(r => r.Description, "parameter_description"),
        ColumnMap<LocalTag>.For(r => r.DataType, "data_type"),
        ColumnMap<LocalTag>.For(r => r.Dimensions.ToSqlFormat(), "dimensions"),
        ColumnMap<LocalTag>.For(r => r.Radix.ToSqlFormat(), "radix"),
        ColumnMap<LocalTag>.For(r => r.Value.ToSqlFormat(), "default_value"),
        ColumnMap<LocalTag>.For(r => r.ExternalAccess?.Name, "external_access"),
        ColumnMap<LocalTag>.For(r => r.Usage?.Name, "tag_usage"),
        ColumnMap<LocalTag>.For(r => r.TagType?.Name, "tag_type"),
        ColumnMap<LocalTag>.For(r => r.AliasFor?.LocalPath, "tag_alias"),
        ColumnMap<LocalTag>.For(_ => false, "is_visible"),
        ColumnMap<LocalTag>.For(_ => false, "is_required"),
        ColumnMap<LocalTag>.For(r => r.Constant, "is_constant"),
        ColumnMap<LocalTag>.RecordHash(this)
    ];
}