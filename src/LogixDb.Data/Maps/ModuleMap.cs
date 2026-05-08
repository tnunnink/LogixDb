using L5Sharp.Core;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "module" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Module"/> class.
/// </summary>
internal class ModuleMap : TableMap<Module>
{
    /// <inheritdoc />
    protected override string TableName => "module";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Module>> Columns =>
    [
        ColumnMap<Module>.For(r => r.Parent?.Hash(), "parent_id"),
        ColumnMap<Module>.For(r => r.Name, "module_name"),
        ColumnMap<Module>.For(r => r.Description, "module_description"),
        ColumnMap<Module>.For(r => r.CatalogNumber, "catalog_number"),
        ColumnMap<Module>.For(r => r.Revision.ToString(), "revision"),
        ColumnMap<Module>.For(r => r.Vendor, "vendor_id"),
        ColumnMap<Module>.For(r => r.ProductType, "product_id"),
        ColumnMap<Module>.For(r => r.ProductCode, "product_code"),
        ColumnMap<Module>.For(r => r.ParentModule, "parent_name"),
        ColumnMap<Module>.For(r => r.ParentModPortId, "parent_port"),
        ColumnMap<Module>.For(r => r.Keying?.Name, "electronic_keying"),
        ColumnMap<Module>.For(r => r.Inhibited, "is_inhibited"),
        ColumnMap<Module>.For(r => r.MajorFault, "is_major_fault_enabled"),
        ColumnMap<Module>.For(r => r.SafetyEnabled, "is_safety_enabled"),
        ColumnMap<Module>.For(r => r.IP?.ToString(), "ip_address"),
        ColumnMap<Module>.For(r => r.Slot, "slot_number"),
        ColumnMap<Module>.For(r => r.Hash(), "record_hash")
    ];
}