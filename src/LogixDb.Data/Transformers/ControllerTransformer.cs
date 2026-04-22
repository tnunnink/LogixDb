using System.Data;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on the controller.
/// </summary>
internal class ControllerTransformer : IDbTransformer
{
    private readonly ControllerMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var records = new List<ControllerRecord> { new(target.InstanceId, source.Controller) };
        yield return _map.GenerateTable(records);
    }
}
