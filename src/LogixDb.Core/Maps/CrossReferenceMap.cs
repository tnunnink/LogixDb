/*using L5Sharp.Core;
using LogixDb.Core.Abstractions;
using LogixDb.Core.Common;

namespace LogixDb.Core.Maps;

public class CrossReferenceMap : TableMap<Reference>
{
    /// <inheritdoc />
    protected override string TableName => "cross_reference";

    /// <inheritdoc />
    public override IReadOnlyList<ColumnMap<Reference>> Columns =>
    [
        ColumnMap<Reference>.For(t => t., "name"),
        ColumnMap<Reference>.For(t => t.Type.Name, "task_type"),
        ColumnMap<Reference>.For(t => t.Description, "description"),
        ColumnMap<Reference>.For(t => t.Priority, "priority"),
        ColumnMap<Reference>.For(t => t.Rate, "rate"),
        ColumnMap<Reference>.For(t => t.Watchdog, "watchdog"),
        ColumnMap<Reference>.For(t => t.InhibitTask, "inhibited"),
        ColumnMap<Reference>.For(t => t.DisableUpdateOutputs, "disable_outputs"),
        ColumnMap<Reference>.For(t => t.EventInfo?.EventTrigger?.Name, "event_trigger"),
        ColumnMap<Reference>.For(t => t.EventInfo?.EventTag?.LocalPath, "event_tag"),
        ColumnMap<Reference>.For(t => t.EventInfo?.EnableTimeout, "enable_timeout")
    ];
}*/