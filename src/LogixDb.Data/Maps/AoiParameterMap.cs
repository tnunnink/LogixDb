using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

public class AoiParameterMap : TableMap<Parameter>
{
    /// <inheritdoc />
    protected override string TableName => "aoi_parameter";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Parameter>> Columns =>
    [
        ColumnMap<Parameter>.For(r => r.Parent?.Metadata.Get<string>("record_hash"), "aoi_hash", hashable: false),
        ColumnMap<Parameter>.For(r => r.Name, "parameter_name"),
        ColumnMap<Parameter>.For(r => r.Description, "parameter_description"),
        ColumnMap<Parameter>.For(r => r.DataType, "data_type"),
        ColumnMap<Parameter>.For(r => r.Dimensions.ToSqlFormat(), "dimensions"),
        ColumnMap<Parameter>.For(r => r.Radix.ToSqlFormat(), "radix"),
        ColumnMap<Parameter>.For(r => r.Default?.ToSqlFormat(), "default_value"),
        ColumnMap<Parameter>.For(r => r.ExternalAccess?.Name, "external_access"),
        ColumnMap<Parameter>.For(r => r.Usage, "tag_usage"),
        ColumnMap<Parameter>.For(r => r.TagType?.Name, "tag_type"),
        ColumnMap<Parameter>.For(r => r.AliasFor?.LocalPath, "tag_alias"),
        ColumnMap<Parameter>.For(r => r.Visible, "is_visible"),
        ColumnMap<Parameter>.For(r => r.Required, "is_required"),
        ColumnMap<Parameter>.For(r => r.Constant, "is_constant"),
        ColumnMap<Parameter>.RecordHash(this)
    ];
}