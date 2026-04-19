using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents the mapping configuration for the "aoi_rung" database table.
/// Provides metadata about the table name and its associated column mappings.
/// This class is used to define how the data is structured and translated into the database.
/// </summary>
internal class AoiRungMap : TableMap<AoiRungRecord>
{
    /// <inheritdoc />
    protected override string TableName => "aoi_rung";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<AoiRungRecord>> Columns =>
    [
        ColumnMap<AoiRungRecord>.For(r => r.RungId, "rung_id"),
        ColumnMap<AoiRungRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<AoiRungRecord>.For(r => r.AoiId, "aoi_id"),
        ColumnMap<AoiRungRecord>.For(r => r.RoutineName, "routine_name"),
        ColumnMap<AoiRungRecord>.For(r => r.Rung.Number, "rung_number"),
        ColumnMap<AoiRungRecord>.For(r => r.Rung.Comment, "rung_comment"),
        ColumnMap<AoiRungRecord>.For(r => r.Rung.Text, "rung_text"),
        ColumnMap<AoiRungRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

internal record AoiRungRecord(int SnapshotId, Guid AoiId, string RoutineName, Rung Rung)
{
    public Guid RungId { get; } = Guid.NewGuid();
}