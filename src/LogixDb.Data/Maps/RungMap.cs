using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "rung" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Rung"/> class.
/// </summary>
public class RungMap : TableMap<RungRecord>
{
    /// <inheritdoc />
    public override string TableName => "rung";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<RungRecord>> Columns =>
    [
        ColumnMap<RungRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<RungRecord>.For(r => r.Rung.Scope.Container, "program_name"),
        ColumnMap<RungRecord>.For(r => r.Rung.Routine?.Name, "routine_name"),
        ColumnMap<RungRecord>.For(r => r.Rung.Number, "rung_number"),
        ColumnMap<RungRecord>.For(r => r.Rung.Comment, "rung_comment"),
        ColumnMap<RungRecord>.For(r => r.Rung.Text, "rung_text"),
        ColumnMap<RungRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for a rung entity.
/// This record contains the metadata and code for a specific Logix rung,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this rung record belongs.</param>
/// <param name="Rung">The Logix rung entity containing its code and metadata.</param>
public record RungRecord(int SnapshotId, Rung Rung);