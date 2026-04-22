using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Transforms a <see cref="Target"/> object into a collection of <see cref="DataTable"/> instances
/// containing Add-On Instruction (AOI) operand data. Extracts the AOI name as the primary operand,
/// followed by all required parameters with their names, data types, descriptions, and output status.
/// </summary>
public class OperandTransformer : IDbTransformer
{
    private readonly AoiOperandMap _map = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var instanceId = target.InstanceId;
        var records = new List<AoiOperandRecord>();

        foreach (var aoi in source.AddOnInstructions)
        {
            byte index = 0;
            records.Add(new AoiOperandRecord(instanceId, aoi.Name, index, aoi.Name, aoi.Name, aoi.Description, true));

            foreach (var parameter in aoi.Parameters.Where(p => p.Required is true))
            {
                index++;

                var record = new AoiOperandRecord(
                    instanceId,
                    aoi.Name,
                    index,
                    parameter.Name,
                    parameter.DataType,
                    parameter.Description,
                    parameter.Usage == TagUsage.InOut || parameter.Usage == TagUsage.Output
                );

                records.Add(record);
            }
        }

        yield return _map.GenerateTable(records);
    }
}
