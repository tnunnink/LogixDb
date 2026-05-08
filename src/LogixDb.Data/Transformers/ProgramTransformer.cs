using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on programs.
/// </summary>
internal class ProgramTransformer : IDbTransformer
{
    private readonly ProgramMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var programs = target.GetSource().Programs
            .Select(p => new { Depth = GetDepth(p), Program = p })
            .OrderBy(x => x.Depth)
            .Select(x => x.Program)
            .ToList();

        yield return _map.GenerateTable(programs);
    }

    /// <summary>
    /// Calculates the depth of a given program in the hierarchy by traversing its parent relationships.
    /// </summary>
    /// <param name="program">The program whose depth in the hierarchy is to be calculated.</param>
    /// <returns>The depth of the program, where a root program has a depth of 0.</returns>
    private static int GetDepth(Program program)
    {
        var current = program;
        var depth = 0;

        while (current.Parent is not null)
        {
            depth++;
            current = current.Parent;
        }

        return depth;
    }
}