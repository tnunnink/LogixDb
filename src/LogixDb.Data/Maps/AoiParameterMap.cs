using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

public class AoiParameterMap : TableMap<ParameterRecord>
{
    /// <inheritdoc />
    protected override string TableName => "aoi_parameter";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<ParameterRecord>> Columns =>
    [
        ColumnMap<ParameterRecord>.For(r => r.AoiHash, "aoi_hash"),
        ColumnMap<ParameterRecord>.For(r => r.Name, "parameter_name"),
        ColumnMap<ParameterRecord>.For(r => r.Description, "parameter_description"),
        ColumnMap<ParameterRecord>.For(r => r.DataType, "data_type"),
        ColumnMap<ParameterRecord>.For(r => r.Dimensions, "dimensions"),
        ColumnMap<ParameterRecord>.For(r => r.Radix, "radix"),
        ColumnMap<ParameterRecord>.For(r => r.Default, "default_value"),
        ColumnMap<ParameterRecord>.For(r => r.ExternalAccess, "external_access"),
        ColumnMap<ParameterRecord>.For(r => r.Usage, "tag_usage"),
        ColumnMap<ParameterRecord>.For(r => r.TagType, "tag_type"),
        ColumnMap<ParameterRecord>.For(r => r.AliasFor, "tag_alias"),
        ColumnMap<ParameterRecord>.For(r => r.Visible, "is_visible"),
        ColumnMap<ParameterRecord>.For(r => r.Required, "is_required"),
        ColumnMap<ParameterRecord>.For(r => r.Constant, "is_constant"),
        ColumnMap<ParameterRecord>.RecordHash(this)
    ];
}

public record ParameterRecord(
    string? AoiHash,
    string Name,
    string? Description,
    string? DataType,
    string? Dimensions,
    string? Radix,
    string? Default,
    string? ExternalAccess,
    string? Usage,
    string? TagType,
    string? AliasFor,
    bool Visible,
    bool Required,
    bool Constant)
{
    public static ParameterRecord FromParameter(Parameter parameter, string aoiHash)
    {
        return new ParameterRecord(
            aoiHash,
            parameter.Name,
            parameter.Description,
            parameter.DataType,
            parameter.Dimension.ToSqlFormat(),
            parameter.Radix.ToSqlFormat(),
            parameter.Default?.ToSqlFormat(),
            parameter.ExternalAccess?.Name,
            parameter.Usage.Name,
            parameter.TagType?.Name,
            parameter.AliasFor?.LocalPath,
            parameter.Visible is true,
            parameter.Required is true,
            parameter.Constant is true
        );
    }

    public static ParameterRecord FromLocalTag(LocalTag tag, string aoiHash)
    {
        return new ParameterRecord(
            aoiHash,
            tag.Name,
            tag.Description,
            tag.DataType,
            tag.Dimensions.ToSqlFormat(),
            tag.Radix.ToSqlFormat(),
            tag.Value.ToSqlFormat(),
            tag.ExternalAccess?.Name,
            tag.Usage?.Name,
            tag.TagType?.Name,
            tag.AliasFor?.LocalPath,
            false,
            false,
            tag.Constant is true
        );
    }
}