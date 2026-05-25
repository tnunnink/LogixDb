using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "data_type_member" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="DataTypeMember"/> class.
/// </summary>
public class DataTypeMemberMap : TableMap<DataTypeMember>
{
    /// <inheritdoc />
    protected override string TableName => "data_type_member";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<DataTypeMember>> Columns =>
    [
        ColumnMap<DataTypeMember>.For(r => r.Parent?.Metadata.Get<string>("record_hash"), "type_hash", hashable: false),
        ColumnMap<DataTypeMember>.For(r => r.Name, "member_name"),
        ColumnMap<DataTypeMember>.For(r => r.Description, "member_description"),
        ColumnMap<DataTypeMember>.For(r => r.Metadata.Get<int>("member_index"), "member_index"),
        ColumnMap<DataTypeMember>.For(r => r.DataType, "data_type"),
        ColumnMap<DataTypeMember>.For(r => r.Dimension.ToSqlFormat(), "dimensions"),
        ColumnMap<DataTypeMember>.For(r => r.Radix.ToSqlFormat(), "radix"),
        ColumnMap<DataTypeMember>.For(r => r.ExternalAccess?.Name, "external_access"),
        ColumnMap<DataTypeMember>.For(r => r.Hidden, "is_hidden"),
        ColumnMap<DataTypeMember>.For(r => r.Target, "target_name"),
        ColumnMap<DataTypeMember>.For(r => r.GetBitNumber(), "bit_number"),
        ColumnMap<DataTypeMember>.RecordHash(this)
    ];
}