using L5Sharp.Core;
using LogixDb.Data.Abstractions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "data_type" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="DataType"/> class.
/// </summary>
public class DataTypeMap : TableMap<DataTypeRecord>
{
    /// <inheritdoc />
    public override string TableName => "data_type";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<DataTypeRecord>> Columns =>
    [
        ColumnMap<DataTypeRecord>.For(r => r.SnapshotId, "snapshot_id", false),
        ColumnMap<DataTypeRecord>.For(r => r.DataType.Name, "type_name"),
        ColumnMap<DataTypeRecord>.For(r => r.DataType.Class.Name, "type_class"),
        ColumnMap<DataTypeRecord>.For(r => r.DataType.Family.Name, "type_family"),
        ColumnMap<DataTypeRecord>.For(r => r.DataType.Description, "description"),
        ColumnMap<DataTypeRecord>.For(ComputeHash, "record_hash", false)
    ];
}

/// <summary>
/// Represents a database record for a data type entity.
/// This record contains the metadata for a specific Logix data type,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this data type record belongs.</param>
/// <param name="DataType">The Logix data type entity.</param>
public record DataTypeRecord(int SnapshotId, DataType DataType);