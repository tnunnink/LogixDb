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
    private readonly AoiRungMap _rungMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var instructions = new List<AddOnInstruction>();
        var parameters = new List<ParameterRecord>();
        var rungs = new List<AoiRungRecord>();

        foreach (var aoi in source.AddOnInstructions)
        {
            var aoiHash = _aoiMap.ComputeHash(aoi);
            instructions.Add(aoi);
            parameters.AddRange(aoi.Parameters.Select(p => ParameterRecord.FromParameter(p, aoiHash)));

            // Only attempt to process local tags and logic/rungs if the AOI is not encrypted. 
            if (!aoi.IsEncrypted)
            {
                parameters.AddRange(aoi.LocalTags.Select(t => ParameterRecord.FromLocalTag(t, aoiHash)));

                rungs.AddRange(
                    aoi.Routines
                        .Where(r => r.Type == RoutineType.RLL)
                        .SelectMany(r => r.Rungs)
                        .Select(r => AoiRungRecord.FromRung(r, aoiHash))
                );
            }
        }

        yield return _aoiMap.GenerateTable(instructions);
        yield return _parameterMap.GenerateTable(parameters);
        yield return _rungMap.GenerateTable(rungs);
    }
}