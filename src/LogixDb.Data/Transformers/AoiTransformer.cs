using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on Add-On Instructions (AOI).
/// </summary>
internal class AoiTransformer : ISnapshotTransformer
{
    private readonly AoiMap _aoiMap = new();
    private readonly AoiParameterMap _parameterMap = new();
    private readonly AoiLocalTagMap _localTagMap = new();
    private readonly AoiRungMap _rungMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Snapshot snapshot)
    {
        var source = snapshot.GetSource();
        var aoiRecords = new List<AoiRecord>();
        var parameterRecords = new List<AoiParameterRecord>();
        var localTagRecords = new List<AoiLocalTagRecord>();
        var rungRecords = new List<AoiRungRecord>();

        foreach (var aoi in source.AddOnInstructions)
        {
            var aoiRecord = new AoiRecord(snapshot.SnapshotId, aoi);
            aoiRecords.Add(aoiRecord);

            parameterRecords.AddRange(aoi.Parameters.Select(p =>
                new AoiParameterRecord(snapshot.SnapshotId, aoiRecord.AoiId, p))
            );

            // Only attempt to process local tags and logic/rungs if the AOI is not encrypted. 
            if (!aoi.IsEncrypted)
            {
                localTagRecords.AddRange(aoi.LocalTags.Select(t =>
                    new AoiLocalTagRecord(snapshot.SnapshotId, aoiRecord.AoiId, t))
                );

                rungRecords.AddRange(aoi.Routines.Where(r => r.Type == RoutineType.RLL).SelectMany(r =>
                    r.Rungs.Select(rung => new AoiRungRecord(snapshot.SnapshotId, aoiRecord.AoiId, r.Name, rung)))
                );
            }
        }

        yield return _aoiMap.GenerateTable(aoiRecords);
        yield return _parameterMap.GenerateTable(parameterRecords);
        yield return _localTagMap.GenerateTable(localTagRecords);
        yield return _rungMap.GenerateTable(rungRecords);
    }
}