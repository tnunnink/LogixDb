using L5Sharp.Core;
using LogixDb.Data.Extensions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "aoi" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="AddOnInstruction"/> class.
/// </summary>
internal class AoiMap : TableMap<AddOnInstruction>
{
    /// <inheritdoc />
    protected override string TableName => "aoi";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<AddOnInstruction>> Columns =>
    [
        ColumnMap<AddOnInstruction>.For(r => r.Name, "aoi_name"),
        ColumnMap<AddOnInstruction>.For(r => r.Description, "aoi_description"),
        ColumnMap<AddOnInstruction>.For(r => r.Revision?.ToString(), "aoi_revision"),
        ColumnMap<AddOnInstruction>.For(r => r.RevisionExtension, "aoi_revision_extension"),
        ColumnMap<AddOnInstruction>.For(r => r.RevisionNote, "aoi_revision_note"),
        ColumnMap<AddOnInstruction>.For(r => r.Vendor, "aoi_vendor"),
        ColumnMap<AddOnInstruction>.For(r => r.AdditionalHelpText, "aoi_help_text"),
        ColumnMap<AddOnInstruction>.For(r => r.CreatedDate, "created_date"),
        ColumnMap<AddOnInstruction>.For(r => r.CreatedBy, "created_by"),
        ColumnMap<AddOnInstruction>.For(r => r.EditedDate, "edited_date"),
        ColumnMap<AddOnInstruction>.For(r => r.EditedBy, "edited_by"),
        ColumnMap<AddOnInstruction>.For(r => r.SoftwareRevision?.ToString(), "software_revision"),
        ColumnMap<AddOnInstruction>.For(r => r.ExecutePreScan, "execute_pre_scan"),
        ColumnMap<AddOnInstruction>.For(r => r.ExecutePostScan, "execute_post_scan"),
        ColumnMap<AddOnInstruction>.For(r => r.ExecuteEnableInFalse, "execute_enable_in_false"),
        ColumnMap<AddOnInstruction>.For(r => r.IsEncrypted, "is_encrypted"),
        ColumnMap<AddOnInstruction>.For(r => r.SignatureID, "signature_id"),
        ColumnMap<AddOnInstruction>.For(r => r.SignatureTimestamp, "signature_timestamp"),
        ColumnMap<AddOnInstruction>.For(r => r.Class?.Name, "component_class"),
        ColumnMap<AddOnInstruction>.For(r => r.Hash(), "record_hash")
    ];
}