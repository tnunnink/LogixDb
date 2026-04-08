using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on modules.
/// </summary>
internal class ModuleTransformer : ISnapshotTransformer
{
    private readonly ModuleMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var records = new Dictionary<string, ModuleRecord>(StringComparer.OrdinalIgnoreCase);

        foreach (var module in source.Modules)
        {
            // Some module elements don't have a name (VFD peripherals) not sure if these are worth adding or not.
            if (string.IsNullOrEmpty(module.Name)) continue;

            var parentName = module.ParentModule ?? string.Empty;
            var parent = records.GetValueOrDefault(parentName);
            var record = new ModuleRecord(snapshot.SnapshotId, parent?.ModuleId, module);

            if (!records.TryAdd(record.Module.Name, record))
                throw new InvalidOperationException(
                    $"Duplicate module name encountered: '{record.Module.Name}'. Each module must have a unique name within the snapshot.");

            records.Add(module.Name, record);
        }

        yield return _map.GenerateTable(records.Values);
    }
}