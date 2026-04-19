using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on routines.
/// </summary>
internal class RoutineTransformer : ISnapshotTransformer
{
    private readonly RoutineMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var records = new List<RoutineRecord>();

        foreach (var routine in source.Programs.SelectMany(p => p.Routines))
        {
            var programId = routine.Program?.Metadata.Get<Guid>("id");
            var record = new RoutineRecord(snapshot.SnapshotId, programId, routine);
            routine.Metadata.Add("id", record.RoutineId);
            records.Add(record);
        }

        yield return _map.GenerateTable(records);
    }
}