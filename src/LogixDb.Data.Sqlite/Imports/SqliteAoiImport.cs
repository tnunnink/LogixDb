using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of Add-On Instruction (AOI) records from a LogixDb snapshot into an SQLite database.
/// This class processes AOI entities by querying them from the snapshot source and inserting
/// them into the database using the configured AOI table mapping.
/// </summary>
internal class SqliteAoiImport : SqliteImport
{
    private readonly AoiMap _aoiMap = new();
    private readonly AoiParameterMap _parameterMap = new();
    private readonly AoiLocalTagMap _tagMap = new();
    private readonly AoiOperandMap _operandMap = new();
    private readonly AoiRungMap _rungMap = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var aoiRecords = new List<AoiRecord>();
        var parameterRecords = new List<AoiParameterRecord>();
        var tagRecords = new List<AoiLocalTagRecord>();
        var operandRecords = new List<AoiOperandRecord>();
        var rungRecords = new List<AoiRungRecord>();

        foreach (var aoi in source.AddOnInstructions)
        {
            if (aoi.IsEncrypted)
            {
                //todo Should this be skipped always or be specified through an option? Even if encypted we can still get some aoi info and parameters/operands
                //todo We need some visibility here (logging)
                continue;
            }

            aoiRecords.Add(new AoiRecord(snapshot.SnapshotId, aoi));
            ProcessParameters(snapshot.SnapshotId, aoi, parameterRecords);
            ProcessLocalTags(snapshot.SnapshotId, aoi, tagRecords);
            ProcessOperands(snapshot.SnapshotId, aoi, operandRecords);
            ProcessRungs(snapshot.SnapshotId, aoi, rungRecords);
        }

        await ImportRecords(aoiRecords, _aoiMap, session, token);
        await ImportRecords(parameterRecords, _parameterMap, session, token);
        await ImportRecords(tagRecords, _tagMap, session, token);
        await ImportRecords(operandRecords, _operandMap, session, token);
        await ImportRecords(rungRecords, _rungMap, session, token);
    }

    /// <summary>
    /// Processes the parameters of an Add-On Instruction (AOI) by creating parameter records and adding them to the provided record list.
    /// </summary>
    /// <param name="snapshotId">The identifier of the snapshot being processed.</param>
    /// <param name="aoi">The Add-On Instruction (AOI) entity whose parameters are being processed.</param>
    /// <param name="records">The list where the generated parameter records will be added.</param>
    private static void ProcessParameters(int snapshotId, AddOnInstruction aoi, List<AoiParameterRecord> records)
    {
        foreach (var parameter in aoi.Parameters)
        {
            var record = new AoiParameterRecord(snapshotId, aoi.Name, parameter);
            records.Add(record);
        }
    }

    /// <summary>
    /// Processes the local tags of an Add-On Instruction (AOI) by creating records for each local tag
    /// and adding them to the provided record list.
    /// </summary>
    /// <param name="snapshotId">The identifier of the snapshot being processed.</param>
    /// <param name="aoi">The Add-On Instruction (AOI) entity whose local tags are being processed.</param>
    /// <param name="records">The list where the generated local tag records will be added.</param>
    private static void ProcessLocalTags(int snapshotId, AddOnInstruction aoi, List<AoiLocalTagRecord> records)
    {
        foreach (var localTag in aoi.LocalTags)
        {
            var record = new AoiLocalTagRecord(snapshotId, aoi.Name, localTag);
            records.Add(record);
        }
    }

    /// <summary>
    /// Processes the operands of an Add-On Instruction (AOI) by extracting required parameters and generating operand records.
    /// </summary>
    /// <param name="snapshotId">The identifier of the snapshot being processed.</param>
    /// <param name="aoi">The Add-On Instruction (AOI) entity whose operands are being processed.</param>
    /// <param name="records">The list where the generated operand records will be added.</param>
    private static void ProcessOperands(int snapshotId, AddOnInstruction aoi, List<AoiOperandRecord> records)
    {
        byte index = 0;
        records.Add(new AoiOperandRecord(snapshotId, aoi.Name, index, aoi.Name, aoi.Name, aoi.Description, true));

        foreach (var parameter in aoi.Parameters.Where(p => p.Required is true))
        {
            index++;

            var record = new AoiOperandRecord(
                snapshotId,
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

    /// <summary>
    /// Processes the rungs of an Add-On Instruction (AOI) by extracting rung data
    /// from RLL routines and generating records for each rung.
    /// </summary>
    /// <param name="snapshotId">The identifier of the snapshot being processed.</param>
    /// <param name="aoi">The Add-On Instruction (AOI) entity containing the routines and rungs to process.</param>
    /// <param name="records">The list where the generated rung records will be added.</param>
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