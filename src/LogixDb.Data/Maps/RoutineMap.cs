using L5Sharp.Core;
using LogixDb.Data.Abstractions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "routine" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Routine"/> class.
/// </summary>
public class RoutineMap : TableMap<RoutineRecord>
{
    /// <inheritdoc />
    public override string TableName => "routine";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<RoutineRecord>> Columns =>
    [
        ColumnMap<RoutineRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<RoutineRecord>.For(r => r.Routine.Scope.Container, "container_name"),
        ColumnMap<RoutineRecord>.For(r => r.Routine.Name, "routine_name"),
        ColumnMap<RoutineRecord>.For(r => r.Routine.Type.Name, "routine_type"),
        ColumnMap<RoutineRecord>.For(r => r.Routine.Description, "description"),
        ColumnMap<RoutineRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];

    /// <inheritdoc />
    public override IEnumerable<RoutineRecord> GetRecords(Snapshot snapshot)
    {
        var source = snapshot.GetSource();

        return source.Query<Routine>()
            .Select(r => new RoutineRecord(snapshot.SnapshotId, r));
    }
}

/// <summary>
/// Represents a database record for a routine entity.
/// This record contains the metadata for a specific Logix routine,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this routine record belongs.</param>
/// <param name="Routine">The Logix routine entity.</param>
public record RoutineRecord(int SnapshotId, Routine Routine);