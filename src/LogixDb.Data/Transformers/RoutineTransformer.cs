using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on routines.
/// </summary>
public class RoutineTransformer : IDbTransformer
{
    private readonly RoutineMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource(scrub: true);
        var routines = source.Query<Routine>().ToList();
        yield return _map.GenerateTable(routines);
    }
}