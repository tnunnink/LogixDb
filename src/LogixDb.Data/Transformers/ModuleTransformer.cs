using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on modules.
/// </summary>
public class ModuleTransformer : IDbTransformer
{
    private readonly ModuleMap _moduleMap = new();
    private readonly ModuleConnectionMap _connectionMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        // todo Some module elements don't have a name (VFD peripherals) not sure if these are worth adding or not.
        var modules = target.GetSource().Modules
            .Where(x => !string.IsNullOrEmpty(x.Name))
            .ToList();

        yield return _moduleMap.GenerateTable(modules);
        yield return _connectionMap.GenerateTable(modules.SelectMany(m => m.Connections).ToList());
    }
}