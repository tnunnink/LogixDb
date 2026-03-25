using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on modules.
/// </summary>
internal class ModuleTransformer : ILogixDbTransformer
{
    private readonly ModuleMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();

        var records = source.Modules
            .Where(m => !string.IsNullOrEmpty(m.Name))
            .Select(m => new ModuleRecord(snapshot.SnapshotId, m));

        yield return _map.GenerateTable(records);
    }
}