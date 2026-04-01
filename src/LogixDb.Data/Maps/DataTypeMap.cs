using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "data_type" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="DataType"/> class.
/// </summary>
public class DataTypeMap : TableMap<DataTypeRecord>
{
    /// <inheritdoc />
    protected override string TableName => "data_type";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<DataTypeRecord>> Columns =>
    [
        ColumnMap<DataTypeRecord>.For(r => r.TypeId, "type_id", hashable: false),
        ColumnMap<DataTypeRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<DataTypeRecord>.For(r => r.DataType.Name, "type_name"),
        ColumnMap<DataTypeRecord>.For(r => r.DataType.Class.Name, "type_class"),
        ColumnMap<DataTypeRecord>.For(r => r.DataType.Family.Name, "type_family"),
        ColumnMap<DataTypeRecord>.For(r => r.DataType.Description, "type_description"),
        ColumnMap<DataTypeRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for a data type entity.
/// This record contains the metadata for a specific Logix data type,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this data type record belongs.</param>
/// <param name="DataType">The Logix data type entity.</param>
public record DataTypeRecord(int SnapshotId, DataType DataType)
{
    public Guid TypeId { get; } = Guid.NewGuid();
}