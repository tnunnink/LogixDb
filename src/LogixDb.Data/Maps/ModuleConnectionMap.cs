using L5Sharp.Core;

namespace LogixDb.Data.Maps;

public class ModuleConnectionMap : TableMap<Connection>
{
    /// <inheritdoc />
    protected override string TableName => "module_connection";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Connection>> Columns =>
    [
        ColumnMap<Connection>.For(r => r.Name, "connection_name"),
        ColumnMap<Connection>.For(r => r.RPI, "rpi"),
        ColumnMap<Connection>.For(r => r.Type?.Name, "connection_type"),
        ColumnMap<Connection>.For(r => r.Priority?.Name, "connection_priority"),
        ColumnMap<Connection>.For(r => r.InputConnectionType?.Name, "transmission_type"),
        ColumnMap<Connection>.For(r => r.InputProductionTrigger?.Name, "production_trigger"),
        ColumnMap<Connection>.For(r => r.OutputRedundantOwner, "output_redundant_owner"),
        ColumnMap<Connection>.For(r => r.Unicast, "unicast"),
        ColumnMap<Connection>.For(r => r.ProgrammaticallySendEventTrigger, "programatically_send_event_trigger"),
        ColumnMap<Connection>.For(r => r.EventId, "event_id"),
        ColumnMap<Connection>.For(r => r.InputTag?.Name, "input_tag"),
        ColumnMap<Connection>.For(r => r.InputSize, "input_size"),
        ColumnMap<Connection>.For(r => r.InputTagSuffix, "input_suffix"),
        ColumnMap<Connection>.For(r => r.InputTag?.Name, "output_tag"),
        ColumnMap<Connection>.For(r => r.OutputSize, "output_size"),
        ColumnMap<Connection>.For(r => r.OutputTagSuffix, "output_suffix"),
        ColumnMap<Connection>.For(r => r.ConnectionPath, "connection_path"),
        ColumnMap<Connection>.RecordHash(this)
    ];
}