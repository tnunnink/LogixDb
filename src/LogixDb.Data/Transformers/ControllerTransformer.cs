using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on the controller.
/// </summary>
public class ControllerTransformer : IDbTransformer
{
    private readonly ControllerMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource(scrub: true);
        yield return _map.GenerateTable([source.Controller]);
    }
}