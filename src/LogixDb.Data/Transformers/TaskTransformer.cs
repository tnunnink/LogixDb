using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on tasks.
/// </summary>
public class TaskTransformer : IDbTransformer
{
    private readonly TaskMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var tasks = target.GetSource().Tasks.ToList();
        yield return _map.GenerateTable(tasks);
    }
}