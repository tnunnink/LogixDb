using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "controller" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Controller"/> class.
/// </summary>
public class ControllerMap : TableMap<ControllerRecord>
{
    /// <inheritdoc />
    public override string TableName => "controller";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<ControllerRecord>> Columns =>
    [
        ColumnMap<ControllerRecord>.For(r => r.ControllerId, "controller_id", hashable: false),
        ColumnMap<ControllerRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<ControllerRecord>.For(r => r.Controller.Name, "controller_name"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.ProcessorType, "catalog_number"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.Revision?.ToString(), "controller_revision"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.Description, "controller_description"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.ProjectCreationDate, "project_creation_date"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.LastModifiedDate, "last_modified_date"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.CommPath, "communication_path"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.SFCExecutionControl?.Name, "sfc_execution_control"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.SFCRestartPosition?.Name, "sfc_restart_position"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.SFCLastScan?.Name, "sfc_last_scan"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.ProjectSN, "project_serial_number"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.MatchProjectToController, "match_project_to_controller"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.InhibitAutomaticFirmwareUpdate, "inhibit_firmware_updates"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.CanUseRPIFromProducer, "allow_rfi_from_producer"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.PassThroughConfiguration?.Name, "pass_through_option"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.DownloadProjectDocumentationAndExtendedProperties, "download_documentation"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.DownloadProjectCustomProperties, "download_properties"),
        ColumnMap<ControllerRecord>.For(r => r.Controller.EtherNetIPMode, "ethernet_ip_mode"),
        ColumnMap<ControllerRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for a controller entity.
/// This record contains the metadata and configuration for a specific Logix controller,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this controller record belongs.</param>
/// <param name="Controller">The Logix controller entity containing its configuration.</param>
public record ControllerRecord(int SnapshotId, Controller Controller)
{
    public Guid ControllerId { get; } = Guid.NewGuid();
}