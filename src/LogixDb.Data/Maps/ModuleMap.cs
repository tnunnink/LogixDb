using L5Sharp.Core;
using LogixDb.Data.Abstractions;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "module" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Module"/> class.
/// </summary>
public class ModuleMap : TableMap<ModuleRecord>
{
    /// <inheritdoc />
    public override string TableName => "module";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<ModuleRecord>> Columns =>
    [
        ColumnMap<ModuleRecord>.For(r => r.SnapshotId, "snapshot_id", false),
        ColumnMap<ModuleRecord>.For(r => r.Module.Name, "module_name"),
        ColumnMap<ModuleRecord>.For(r => r.Module.CatalogNumber, "catalog_number"),
        ColumnMap<ModuleRecord>.For(r => r.Module.Revision?.ToString(), "revision"),
        ColumnMap<ModuleRecord>.For(r => r.Module.Description, "description"),
        ColumnMap<ModuleRecord>.For(r => r.Module.Vendor?.Id ?? 0, "vendor_id"),
        ColumnMap<ModuleRecord>.For(r => r.Module.ProductType?.Id ?? 0, "product_id"),
        ColumnMap<ModuleRecord>.For(r => r.Module.ProductCode, "product_code"),
        ColumnMap<ModuleRecord>.For(r => r.Module.ParentModule, "parent_name"),
        ColumnMap<ModuleRecord>.For(r => r.Module.ParentModPortId, "parent_port"),
        ColumnMap<ModuleRecord>.For(r => r.Module.Keying?.Name, "electronic_keying"),
        ColumnMap<ModuleRecord>.For(r => r.Module.Inhibited, "inhibited"),
        ColumnMap<ModuleRecord>.For(r => r.Module.MajorFault, "major_fault"),
        ColumnMap<ModuleRecord>.For(r => r.Module.SafetyEnabled, "safety_enabled"),
        ColumnMap<ModuleRecord>.For(r => r.Module.IP?.ToString(), "ip_address"),
        ColumnMap<ModuleRecord>.For(r => r.Module.Slot, "slot_number"),
        ColumnMap<ModuleRecord>.For(ComputeHash, "record_hash", false)
    ];
}

/// <summary>
/// Represents a database record for a module entity.
/// This record contains the metadata for a specific Logix module,
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this module record belongs.</param>
/// <param name="Module">The Logix module entity.</param>
public record ModuleRecord(int SnapshotId, Module Module);