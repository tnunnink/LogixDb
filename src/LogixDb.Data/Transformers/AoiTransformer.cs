using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Snapshot"/> object into a collection of
/// <see cref="DataTable"/> instances focused on Add-On Instructions (AOI).
/// </summary>
internal class AoiTransformer : ILogixDbTransformer
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
            aoiRecords.Add(new AoiRecord(snapshot.SnapshotId, aoi));
            ProcessParameters(snapshot.SnapshotId, aoi, parameterRecords);

            // Only attempt to process local tags and logic/rungs if the AOI is not encrypted
            if (!aoi.IsEncrypted)
            {
                ProcessLocalTags(snapshot.SnapshotId, aoi, localTagRecords);
                ProcessRungs(snapshot.SnapshotId, aoi, rungRecords);
            }
        }

        yield return _aoiMap.GenerateTable(aoiRecords);
        yield return _parameterMap.GenerateTable(parameterRecords);
        yield return _localTagMap.GenerateTable(localTagRecords);
        yield return _rungMap.GenerateTable(rungRecords);
    }

    private static void ProcessParameters(int snapshotId, AddOnInstruction aoi, List<AoiParameterRecord> records)
    {
        foreach (var parameter in aoi.Parameters)
        {
            var record = new AoiParameterRecord(snapshotId, aoi.Name, parameter);
            records.Add(record);
        }
    }

    private static void ProcessLocalTags(int snapshotId, AddOnInstruction aoi, List<AoiLocalTagRecord> records)
    {
        foreach (var localTag in aoi.LocalTags)
        {
            var record = new AoiLocalTagRecord(snapshotId, aoi.Name, localTag);
            records.Add(record);
        }
    }

    private static void ProcessRungs(int snapshotId, AddOnInstruction aoi, List<AoiRungRecord> records)
    {
        var routines = aoi.Routines.Where(r => r.Type == RoutineType.RLL);

        foreach (var routine in routines)
        {
            var rungs = routine.Rungs.Select(r => new AoiRungRecord(snapshotId, aoi.Name, routine.Name, r));
            records.AddRange(rungs);
        }
    }
}