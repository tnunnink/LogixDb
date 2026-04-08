using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on the controller.
/// </summary>
internal class ControllerTransformer : ISnapshotTransformer
{
    private readonly ControllerMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var records = new List<ControllerRecord> { new(snapshot.SnapshotId, source.Controller) };
        yield return _map.GenerateTable(records);
    }
}