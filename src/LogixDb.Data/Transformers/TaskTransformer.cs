using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on tasks.
/// </summary>
internal class TaskTransformer : IDbTransformer
{
    private readonly TaskMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var records = new List<TaskRecord>();

        foreach (var task in source.Tasks)
        {
            var record = new TaskRecord(target.InstanceId, task);
            task.Metadata.Add("id", record.TaskId);
            records.Add(record);
        }

        yield return _map.GenerateTable(records);
    }
}
