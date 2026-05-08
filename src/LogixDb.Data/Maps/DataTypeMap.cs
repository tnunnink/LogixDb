using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "data_type" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="DataType"/> class.
/// </summary>
internal class DataTypeMap : TableMap<DataType>
{
    /// <inheritdoc />
    protected override string TableName => "data_type";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<DataType>> Columns =>
    [
        ColumnMap<DataType>.For(r => r.Name, "type_name"),
        ColumnMap<DataType>.For(r => r.Description, "type_description"),
        ColumnMap<DataType>.For(r => r.Class.Name, "type_class"),
        ColumnMap<DataType>.For(r => r.Family.Name, "type_family"),
        ColumnMap<DataType>.For(r => r.Hash(), "record_hash")
    ];
}