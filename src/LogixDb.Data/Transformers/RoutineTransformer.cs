using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on routines.
/// </summary>
internal class RoutineTransformer : ILogixDbTransformer
{
    private readonly RoutineMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var records = source.Query<Routine>().Select(x => new RoutineRecord(snapshot.SnapshotId, x));
        yield return _map.GenerateTable(records);
    }
}