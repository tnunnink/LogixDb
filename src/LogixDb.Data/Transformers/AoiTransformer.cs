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

            ProcessParameters(aoiRecord, parameterRecords);

            // Only attempt to process local tags and logic/rungs if the AOI is not encrypted
            if (!aoi.IsEncrypted)
            {
                ProcessLocalTags(aoiRecord, localTagRecords);
                ProcessRungs(aoiRecord, rungRecords);
            }
        }

        yield return _aoiMap.GenerateTable(aoiRecords);
        yield return _parameterMap.GenerateTable(parameterRecords);
        yield return _localTagMap.GenerateTable(localTagRecords);
        yield return _rungMap.GenerateTable(rungRecords);
    }

    /// <summary>
    /// Processes the parameters of an Add-On Instruction (AOI) and adds them as records to the provided collection.
    /// </summary>
    /// <param name="parent">The parent AOI record containing details of the AOI.</param>
    /// <param name="records">The collection where the processed AOI parameter records will be added.</param>
    private static void ProcessParameters(AoiRecord parent, List<AoiParameterRecord> records)
    {
        foreach (var parameter in parent.Aoi.Parameters)
        {
            var record = new AoiParameterRecord(parent.AoiId, parameter);
            records.Add(record);
        }
    }

    /// <summary>
    /// Processes the collection of local tags defined within a specified Add-On Instruction (AOI) and
    /// converts them into a list of <see cref="AoiLocalTagRecord"/> records for persistence.
    /// </summary>
    /// <param name="parent">The <see cref="AoiRecord"/> representing the parent Add-On Instruction.</param>
    /// <param name="records">The collection where generated <see cref="AoiLocalTagRecord"/> instances will be added.</param>
    private static void ProcessLocalTags(AoiRecord parent, List<AoiLocalTagRecord> records)
    {
        foreach (var localTag in parent.Aoi.LocalTags)
        {
            var record = new AoiLocalTagRecord(parent.AoiId, localTag);
            records.Add(record);
        }
    }

    /// <summary>
    /// Processes the rungs within the routines of a given AOI and adds them as records to the provided collection.
    /// </summary>
    /// <param name="parent">The parent AOI record associated with the routines and their rungs.</param>
    /// <param name="records">The collection of rung records to which processed rungs will be added.</param>
    private static void ProcessRungs(AoiRecord parent, List<AoiRungRecord> records)
    {
        var routines = parent.Aoi.Routines.Where(r => r.Type == RoutineType.RLL);

        foreach (var routine in routines)
        {
            var rungs = routine.Rungs.Select(r => new AoiRungRecord(parent.AoiId, routine.Name, r));
            records.AddRange(rungs);
        }
    }
}