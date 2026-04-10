using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "data_type_member" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="DataTypeMember"/> class.
/// </summary>
internal class DataTypeMemberMap : TableMap<DataTypeMemberRecord>
{
    /// <inheritdoc />
    protected override string TableName => "data_type_member";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<DataTypeMemberRecord>> Columns =>
    [
        ColumnMap<DataTypeMemberRecord>.For(r => r.MemberId, "member_id", hashable: false),
        ColumnMap<DataTypeMemberRecord>.For(r => r.TypeId, "type_id", hashable: false),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Name, "member_name", hashable: false),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Description, "member_description"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.DataType, "data_type"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Dimension.ToSqlFormat(), "dimensions"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Radix.ToSqlFormat(), "radix"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.ExternalAccess?.Name, "external_access"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Hidden, "is_hidden"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Target, "target_name"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.GetBitNumber(), "bit_number"),
        ColumnMap<DataTypeMemberRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for a data type member entity.
/// This record contains the metadata for a specific member of a Logix data type,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
internal record DataTypeMemberRecord(Guid TypeId, DataTypeMember Member)
{
    public Guid MemberId { get; } = Guid.NewGuid();
}