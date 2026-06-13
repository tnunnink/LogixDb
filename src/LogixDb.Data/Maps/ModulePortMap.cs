using L5Sharp.Core;

namespace LogixDb.Data.Maps;

public class ModulePortMap : TableMap<Port>
{
    /// <inheritdoc />
    protected override string TableName => "module_port";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Port>> Columns =>
    [
        ColumnMap<Port>.For(p => p.GetParent<Module>()?.Metadata.Get<string>("record_hash"), "module_hash", false),
        ColumnMap<Port>.For(p => p.Id, "port_number"),
        ColumnMap<Port>.For(p => p.Type, "port_type"),
        ColumnMap<Port>.For(p => p.Address?.ToString(), "address"),
        ColumnMap<Port>.For(p => p.Upstream, "upstream"),
        ColumnMap<Port>.For(p => p.BusSize, "bus_size"),
        ColumnMap<Port>.For(p => p.IsChassis, "bus_size"),
        ColumnMap<Port>.RecordHash(this)
    ];
}