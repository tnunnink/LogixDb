using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for LocalTag objects to the "aoi_parameter" database table.
/// This class defines how specific properties of LocalTag elements are mapped to corresponding table columns.
/// </summary>
/// <remarks>
/// This is my way of combining the local tags and parameters for an AOI into a single table in the database.
/// They are essentially all the same columns, except that the local tag doesn't have required and visible properties.
/// I'd prefer not to make separate tables and not to tread these as "tag" instances but as member definitions.
/// </remarks>
public class AoiLocalTagMap : TableMap<AoiLocalTagRecord>
{
    /// <inheritdoc />
    public override string TableName => "aoi_parameter";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<AoiLocalTagRecord>> Columns =>
    [
        ColumnMap<AoiLocalTagRecord>.For(r => r.SnapshotId, "snapshot_id", false),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Instruction?.Name, "aoi_name"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Name, "parameter_name"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.GetDataTypeName(), "data_type"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Value.IsAtomic() ? r.Tag.Value.ToString() : null, "default_value"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Description, "description"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.ExternalAccess?.Name, "external_access"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Usage?.Name, "tag_usage"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.TagType?.Name, "tag_type"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.AliasFor?.LocalPath, "tag_alias"),
        ColumnMap<AoiLocalTagRecord>.For(r => r.Tag.Constant, "constant"),
        ColumnMap<AoiLocalTagRecord>.For(ComputeHash, "record_hash", false)
    ];

    /// <inheritdoc />
    public override IEnumerable<AoiLocalTagRecord> GetRecords(Snapshot snapshot)
    {
        var source = snapshot.GetSource();

        return source.AddOnInstructions
            .SelectMany(aoi => aoi.LocalTags)
            .Select(t => new AoiLocalTagRecord(snapshot.SnapshotId, t));
    }
}

/// <summary>
/// Represents a database record for an AOI local tag entity.
/// This record contains the metadata for a specific local tag within an AOI,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this local tag record belongs.</param>
/// <param name="Tag">The Logix local tag entity.</param>
public record AoiLocalTagRecord(int SnapshotId, LocalTag Tag);