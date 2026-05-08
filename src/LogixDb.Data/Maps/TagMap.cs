using L5Sharp.Core;

namespace LogixDb.Data.Maps;

internal class TagMap : TableMap<Tag>
{
    /// <inheritdoc />
    protected override string TableName => "tag";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Tag>> Columns =>
    [
        ColumnMap<Tag>.For(r => r.Program?.Hash(), "program_id"),
        ColumnMap<Tag>.For(r => r.Name, "tag_name"),
        ColumnMap<Tag>.For(r => r.DataType, "data_type"),
        ColumnMap<Tag>.For(r => r.Dimensions.ToSqlFormat(), "dimensions"),
        ColumnMap<Tag>.For(r => r.Radix.ToSqlFormat(), "radix"),
        ColumnMap<Tag>.For(r => r.ExternalAccess?.Name, "external_access"),
        ColumnMap<Tag>.For(r => r.OpcUAAccess?.Name ?? Access.None, "opcua_access"),
        ColumnMap<Tag>.For(r => r.Constant ?? false, "is_constant"),
        ColumnMap<Tag>.For(r => r.TagType?.Name ?? TagType.Base, "tag_type"),
        ColumnMap<Tag>.For(r => r.Usage?.Name ?? TagUsage.Normal, "tag_usage"),
        ColumnMap<Tag>.For(r => r.AliasFor?.LocalPath, "alias_for"),
        ColumnMap<Tag>.For(r => r.Hash(), "record_hash")
    ];
}