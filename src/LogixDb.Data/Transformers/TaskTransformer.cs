using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on tasks.
/// </summary>
internal class TaskTransformer : ISnapshotTransformer
{
    private readonly TaskMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var records = new List<TaskRecord>();

        foreach (var task in source.Tasks)
        {
            var record = new TaskRecord(snapshot.SnapshotId, task);
            task.Metadata.Add("id", record.TaskId);
            records.Add(record);
        }

        yield return _map.GenerateTable(records);
    }
}