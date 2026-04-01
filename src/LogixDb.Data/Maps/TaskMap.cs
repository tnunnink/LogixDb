using Task = L5Sharp.Core.Task;

namespace LogixDb.Data.Maps;

/// <summary>
/// Represents a mapping configuration for the "task" table within the database.
/// This class defines the schema of the table, including the table name and the columns
/// that map to the properties of the <see cref="Task"/> class.
/// </summary>
public class TaskMap : TableMap<TaskRecord>
{
    /// <inheritdoc />
    protected override string TableName => "task";

    /// <inheritdoc />
    protected override IReadOnlyList<ColumnMap<TaskRecord>> Columns =>
    [
        ColumnMap<TaskRecord>.For(r => r.TaskId, "task_id", hashable: false),
        ColumnMap<TaskRecord>.For(r => r.SnapshotId, "snapshot_id", hashable: false),
        ColumnMap<TaskRecord>.For(r => r.Task.Name, "task_name"),
        ColumnMap<TaskRecord>.For(r => r.Task.Type.Name, "task_type"),
        ColumnMap<TaskRecord>.For(r => r.Task.Description, "task_description"),
        ColumnMap<TaskRecord>.For(r => r.Task.Priority, "task_priority"),
        ColumnMap<TaskRecord>.For(r => r.Task.Rate, "task_rate"),
        ColumnMap<TaskRecord>.For(r => r.Task.Watchdog, "task_watchdog"),
        ColumnMap<TaskRecord>.For(r => r.Task.InhibitTask, "is_inhibited"),
        ColumnMap<TaskRecord>.For(r => r.Task.DisableUpdateOutputs, "disable_outputs"),
        ColumnMap<TaskRecord>.For(r => r.Task.EventInfo?.EventTrigger?.Name, "event_trigger"),
        ColumnMap<TaskRecord>.For(r => r.Task.EventInfo?.EventTag?.LocalPath, "event_tag"),
        ColumnMap<TaskRecord>.For(r => r.Task.EventInfo?.EnableTimeout, "enable_timeout"),
        ColumnMap<TaskRecord>.For(ComputeHash, "record_hash", hashable: false)
    ];
}

/// <summary>
/// Represents a database record for a task entity.
/// This record contains properties for the task's associated metadata and configuration settings
/// as well as the unique identifier linking it to a specific database snapshot.
/// </summary>
/// <param name="SnapshotId">The unique identifier of the snapshot to which this task record belongs.</param>
/// <param name="Task">The task entity containing metadata, configuration, and execution details.</param>
public record TaskRecord(int SnapshotId, Task Task)
{
    public Guid TaskId { get; } = Guid.NewGuid();
}