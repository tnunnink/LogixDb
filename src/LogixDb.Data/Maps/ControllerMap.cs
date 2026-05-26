using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "controller" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Controller"/> class.
/// </summary>
public class ControllerMap : TableMap<Controller>
{
    /// <inheritdoc />
    protected override string TableName => "controller";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Controller>> Columns =>
    [
        ColumnMap<Controller>.For(r => r.Name, "controller_name"),
        ColumnMap<Controller>.For(r => r.Description, "controller_description"),
        ColumnMap<Controller>.For(r => r.ProcessorType, "catalog_number"),
        ColumnMap<Controller>.For(r => r.Revision.ToString(), "revision"),
        ColumnMap<Controller>.For(r => r.ProjectCreationDate, "project_creation_date"),
        ColumnMap<Controller>.For(r => r.LastModifiedDate, "last_modified_date"),
        ColumnMap<Controller>.For(r => r.CommPath, "communication_path"),
        ColumnMap<Controller>.For(r => r.SFCExecutionControl?.Name, "sfc_execution_control"),
        ColumnMap<Controller>.For(r => r.SFCRestartPosition?.Name, "sfc_restart_position"),
        ColumnMap<Controller>.For(r => r.SFCLastScan?.Name, "sfc_last_scan"),
        ColumnMap<Controller>.For(r => r.ProjectSN, "project_serial_number"),
        ColumnMap<Controller>.For(r => r.MatchProjectToController, "match_project_to_controller"),
        ColumnMap<Controller>.For(r => r.InhibitAutomaticFirmwareUpdate, "inhibit_firmware_updates"),
        ColumnMap<Controller>.For(r => r.CanUseRPIFromProducer, "allow_rfi_from_producer"),
        ColumnMap<Controller>.For(r => r.PassThroughConfiguration?.Name, "pass_through_option"),
        ColumnMap<Controller>.For(r => r.DownloadProjectDocumentationAndExtendedProperties, "download_documentation"),
        ColumnMap<Controller>.For(r => r.DownloadProjectCustomProperties, "download_properties"),
        ColumnMap<Controller>.For(r => r.EtherNetIPMode?.Name, "ethernet_ip_mode"),
        ColumnMap<Controller>.For(r => r.RedundancyInfo?.Enabled, "redundancy_enabled"),
        ColumnMap<Controller>.For(r => r.RedundancyInfo?.KeepTestEditsOnSwitchOver, "keep_test_edits_on_switch"),
        ColumnMap<Controller>.For(r => r.RedundancyInfo?.IOMemoryPadPercentage, "io_memory_pad_percent"),
        ColumnMap<Controller>.For(r => r.RedundancyInfo?.DataTablePadPercentage, "data_table_pad_percent"),
        ColumnMap<Controller>.For(r => r.SafetyInfo?.SafetySignature, "safety_signature"),
        ColumnMap<Controller>.For(r => r.SafetyInfo?.SafetyLockPassword, "safety_lock_password"),
        ColumnMap<Controller>.For(r => r.SafetyInfo?.SafetyUnlockPassword, "safety_unlock_password"),
        ColumnMap<Controller>.For(r => r.SafetyInfo?.ConfigureSafetyIOAlways, "configure_safety_io_always"),
        ColumnMap<Controller>.For(r => r.SafetyInfo?.SignatureRunModeProtect, "signature_run_mode_protect"),
        ColumnMap<Controller>.For(r => r.Security?.Code, "security_code"),
        ColumnMap<Controller>.For(r => r.Security?.SecurityAuthorityID, "security_authority_id"),
        ColumnMap<Controller>.For(r => r.Security?.SecurityAuthorityURI, "security_authority_uri"),
        ColumnMap<Controller>.For(r => r.Security?.PermissionSet, "permission_set"),
        ColumnMap<Controller>.For(r => r.Security?.ChangesToDetect, "changed_to_detect"),
        ColumnMap<Controller>.For(r => r.Security?.TrustedSlots, "trusted_slots"),
        ColumnMap<Controller>.RecordHash(this)
    ];
}