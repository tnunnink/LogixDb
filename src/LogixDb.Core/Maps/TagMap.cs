using L5Sharp.Core;
using LogixDb.Core.Abstractions;
using LogixDb.Core.Common;

namespace LogixDb.Core.Maps;

/// <summary>
/// Represents a mapping configuration for the "tag" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Tag"/> class.
/// </summary>
public class TagMap : TableMap<Tag>
{
    /// <inheritdoc />
    public override string TableName => "tag";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<Tag>> Columns =>
    [
        ColumnMap<Tag>.For(t => t.Reference, "reference"),
        ColumnMap<Tag>.For(t => t.Base.TagName, "base_name"),
        ColumnMap<Tag>.For(t => t.TagName, "tag_name"),
        ColumnMap<Tag>.For(t => t.Scope.Level, "scope_level"),
        ColumnMap<Tag>.For(t => t.Scope.Container, "scope_name"),
        ColumnMap<Tag>.For(t => t.DataType, "data_type")
    ];
}