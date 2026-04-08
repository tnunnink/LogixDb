using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "rung" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Rung"/> class.
/// </summary>
internal class RungMap : TableMap<RungRecord>
{
    /// <inheritdoc />
    protected override string TableName => "rung";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<RungRecord>> Columns =>
    [
        ColumnMap<RungRecord>.For(r => r.RungId, "rung_id", hashable: false),
        ColumnMap<RungRecord>.For(r => r.RoutineId, "routine_id"),
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
internal record RungRecord(Guid? RoutineId, Rung Rung)
{
    public Guid RungId { get; } = Guid.NewGuid();
}