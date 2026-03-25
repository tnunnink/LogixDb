using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on tasks.
/// </summary>
internal class TaskTransformer : ILogixDbTransformer
{
    private readonly TaskMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var records = source.Query<L5Sharp.Core.Task>().Select(t => new TaskRecord(snapshot.SnapshotId, t));
        yield return _map.GenerateTable(records);
    }
}