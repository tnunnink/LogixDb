using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on routines.
/// </summary>
internal class RoutineTransformer : IDbTransformer
{
    private readonly RoutineMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var records = new List<RoutineRecord>();

        foreach (var routine in source.Programs.SelectMany(p => p.Routines))
        {
            var programId = routine.Program?.Metadata.Get<Guid>("id");
            var record = new RoutineRecord(target.InstanceId, programId, routine);
            routine.Metadata.Add("id", record.RoutineId);
            records.Add(record);
        }

        yield return _map.GenerateTable(records);
    }
}
