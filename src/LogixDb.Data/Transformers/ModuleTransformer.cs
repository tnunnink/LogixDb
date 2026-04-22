using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on modules.
/// </summary>
internal class ModuleTransformer : IDbTransformer
{
    private readonly ModuleMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var records = new Dictionary<string, ModuleRecord>(StringComparer.OrdinalIgnoreCase);

        foreach (var module in source.Modules)
        {
            // Some module elements don't have a name (VFD peripherals) not sure if these are worth adding or not.
            if (string.IsNullOrEmpty(module.Name)) continue;

            var parentName = module.ParentModule ?? string.Empty;
            var parent = records.GetValueOrDefault(parentName);
            var record = new ModuleRecord(target.InstanceId, parent?.ModuleId, module);

            if (!records.TryAdd(record.Module.Name, record))
                throw new InvalidOperationException(
                    $"Duplicate module name encountered: '{record.Module.Name}'. Each module must have a unique name within the Target.");
        }

        yield return _map.GenerateTable(records.Values);
    }
}
