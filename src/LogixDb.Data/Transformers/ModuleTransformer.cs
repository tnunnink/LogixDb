using System.Data;
using L5Sharp.Core;
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
        var modules = target.GetSource().Modules
            .Select(x => new { Depth = GetDepth(x), Module = x })
            .OrderBy(x => x.Depth)
            .Select(x => x.Module)
            .ToList();

        yield return _map.GenerateTable(modules);
    }

    /// <summary>
    /// Calculates the depth of the specified module within the module hierarchy.
    /// The depth is determined by traversing the parent modules until the root module is reached.
    /// </summary>
    /// <param name="module">The module for which the depth is to be calculated.</param>
    /// <returns>The depth of the module in the hierarchy as an integer.</returns>
    private static int GetDepth(Module module)
    {
        var current = module;
        var depth = 0;

        while (current.Parent is not null)
        {
            depth++;
            current = current.Parent;
        }

        return depth;
    }
}