using Task = L5Sharp.Core.Task;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "task" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Task"/> class.
/// </summary>
public class TaskMap : TableMap<Task>
{
    /// <inheritdoc />
    protected override string TableName => "task";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<Task>> Columns =>
    [
        ColumnMap<Task>.For(r => r.Name, "task_name"),
        ColumnMap<Task>.For(r => r.Description, "task_description"),
        ColumnMap<Task>.For(r => r.Type.Name, "task_type"),
        ColumnMap<Task>.For(r => r.Priority, "priority"),
        ColumnMap<Task>.For(r => r.Rate, "scan_rate"),
        ColumnMap<Task>.For(r => r.Watchdog, "watchdog"),
        ColumnMap<Task>.For(r => r.InhibitTask, "is_inhibited"),
        ColumnMap<Task>.For(r => r.DisableUpdateOutputs, "disable_outputs"),
        ColumnMap<Task>.For(r => r.EventInfo?.EventTrigger?.Name, "event_trigger"),
        ColumnMap<Task>.For(r => r.EventInfo?.EventTag?.LocalPath, "event_tag"),
        ColumnMap<Task>.For(r => r.EventInfo?.EnableTimeout, "enable_timeout"),
        ColumnMap<Task>.RecordHash(this)
    ];
}