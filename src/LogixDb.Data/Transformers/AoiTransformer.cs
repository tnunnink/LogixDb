using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on Add-On Instructions (AOI).
/// </summary>
public class AoiTransformer : IDbTransformer
{
    private readonly AoiMap _aoiMap = new();
    private readonly AoiParameterMap _parameterMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var instructions = new List<AddOnInstruction>();
        var parameters = new List<Parameter>();

        foreach (var aoi in source.AddOnInstructions)
        {
            instructions.Add(aoi);
            parameters.AddRange(aoi.Parameters);

            // Only attempt to process local tags if the AOI is not encrypted.
            // Routine data is handled by the routine transformer.
            if (!aoi.IsEncrypted)
                parameters.AddRange(aoi.LocalTags);
        }

        yield return _aoiMap.GenerateTable(instructions);
        yield return _parameterMap.GenerateTable(parameters);
    }
}