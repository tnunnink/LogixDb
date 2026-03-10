using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "data_type_member" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="DataTypeMember"/> class.
/// </summary>
public class DataTypeMemberMap : TableMap<DataTypeMemberRecord>
{
    /// <inheritdoc />
    public override string TableName => "data_type_member";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<DataTypeMemberRecord>> Columns =>
    [
        ColumnMap<DataTypeMemberRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Parent?.Name, "type_name"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Name, "member_name"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.DataType, "data_type"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Dimension, "dimension"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Radix?.Name, "radix"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.ExternalAccess?.Name, "external_access"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Description, "member_description"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Hidden, "is_hidden"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.Target, "target_name"),
        ColumnMap<DataTypeMemberRecord>.For(r => r.Member.BitNumber is not null ? (byte)r.Member.BitNumber : null,
            "bit_number"),
        ColumnMap<DataTypeMemberRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];

    /// <inheritdoc />
    public override IEnumerable<DataTypeMemberRecord> GetRecords(Snapshot snapshot)
    {
        var source = snapshot.GetSource();

        return source.Query<DataType>()
            .SelectMany(d => d.Members)
            .Select(m => new DataTypeMemberRecord(snapshot.SnapshotId, m));
    }
}

/// <summary>
/// Represents a database record for a data type member entity.
/// This record contains the metadata for a specific member of a Logix data type,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this member record belongs.</param>
/// <param name="Member">The Logix data type member entity.</param>
public record DataTypeMemberRecord(int SnapshotId, DataTypeMember Member);