using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Extensions;
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
        var programs = target.GetSource().Programs.ToList();
        yield return _map.GenerateTable(programs);
    }
}