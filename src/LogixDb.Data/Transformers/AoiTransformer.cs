using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on Add-On Instructions (AOI).
/// </summary>
internal class AoiTransformer : IDbTransformer
{
    private readonly AoiMap _aoiMap = new();
    private readonly AoiParameterMap _parameterMap = new();
    private readonly AoiLocalTagMap _localTagMap = new();
    private readonly AoiRungMap _rungMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var aoiRecords = new List<AddOnInstruction>();
        var parameterRecords = new List<Parameter>();
        var localTagRecords = new List<AoiLocalTagRecord>();
        var rungRecords = new List<AoiRungRecord>();

        foreach (var aoi in source.AddOnInstructions)
        {
            var aoiHash = aoi.Hash();
            aoiRecords.Add(aoi);
            parameterRecords.AddRange(aoi.Parameters);

            // Only attempt to process local tags and logic/rungs if the AOI is not encrypted. 
            if (!aoi.IsEncrypted)
            {
                localTagRecords.AddRange(aoi.LocalTags.Select(t => new AoiLocalTagRecord(aoiHash, t)));

                var rungs = aoi.Routines.Where(r => r.Type == RoutineType.RLL).SelectMany(r => r.Rungs);
                rungRecords.AddRange(rungs.Select(r => new AoiRungRecord(aoiHash, r)));
            }
        }

        yield return _aoiMap.GenerateTable(aoiRecords);
        yield return _parameterMap.GenerateTable(parameterRecords);
        yield return _localTagMap.GenerateTable(localTagRecords);
        yield return _rungMap.GenerateTable(rungRecords);
    }
}