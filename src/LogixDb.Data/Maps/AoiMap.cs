using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "aoi" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="AddOnInstruction"/> class.
/// </summary>
public class AoiMap : TableMap<AoiRecord>
{
    /// <inheritdoc />
    public override string TableName => "aoi";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<AoiRecord>> Columns =>
    [
        ColumnMap<AoiRecord>.For(r => r.AoiId, "aoi_id", hashable: false),
        ColumnMap<AoiRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<AoiRecord>.For(r => r.Aoi.Name, "aoi_name"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.Revision?.ToString(), "aoi_revision"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.RevisionExtension, "aoi_revision_extension"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.RevisionNote, "aoi_revision_note"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.Vendor, "aoi_vendor"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.Description, "aoi_description"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.ExecutePreScan, "execute_pre_scan"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.ExecutePostScan, "execute_post_scan"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.ExecuteEnableInFalse, "execute_enable_in_false"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.CreatedDate, "created_date"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.CreatedBy, "created_by"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.EditedDate, "edited_date"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.EditedBy, "edited_by"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.SoftwareRevision?.ToString(), "software_revision"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.AdditionalHelpText, "help_text"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.IsEncrypted, "is_encrypted"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.SignatureID, "signature_id"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.SignatureTimestamp, "signature_timestamp"),
        ColumnMap<AoiRecord>.For(r => r.Aoi.Class?.Name, "component_class"),
        ColumnMap<AoiRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for an AOI entity.
/// This record contains metadata and configuration for a specific Logix AOI,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this AOI record belongs.</param>
/// <param name="Aoi">The Logix AOI entity containing its configuration.</param>
public record AoiRecord(int SnapshotId, AddOnInstruction Aoi)
{
    public Guid AoiId { get; } = Guid.NewGuid();
}