using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;
using Task = System.Threading.Tasks.Task;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Handles the import of rung records from a LogixDb snapshot into an SQLite database.
/// This class processes rung entities by querying them from the snapshot source and inserting
/// them into the database using the configured rung table mapping.
/// </summary>
internal class SqliteRungImport : SqliteImport
{
    private readonly RungMap _rungMap = new();
    private readonly InstructionMap _instructionMap = new();
    private readonly ArgumentMap _argumentMap = new();

    public override async Task Process(Snapshot snapshot, ILogixDbSession session, ImportOptions options,
        CancellationToken token)
    {
        var source = snapshot.GetSource();
        var rungRecords = new List<RungRecord>();
        var instructionRecords = new List<InstructionRecord>();
        var argumentRecords = new List<ArgumentRecord>();

        var rungs = source.Programs
            .SelectMany(p => p.Routines.Where(r => r.Type == RoutineType.RLL))
            .SelectMany(r => r.Rungs);

        foreach (var rung in rungs)
        {
            var rungRecord = new RungRecord(snapshot.SnapshotId, rung);
            rungRecords.Add(rungRecord);
            ProcessRung(snapshot.SnapshotId, rungRecord.RungId, rung, instructionRecords, argumentRecords);
        }

        await ImportRecords(rungRecords, _rungMap, session, token);
        await ImportRecords(instructionRecords, _instructionMap, session, token);
        await ImportRecords(argumentRecords, _argumentMap, session, token);
    }

    /// <summary>
    /// Processes a rung by extracting its instructions and their associated arguments,
    /// creating records for both, and computing their respective hashes before adding
    /// them to the provided collections.
    /// </summary>
    /// <param name="snapshotId">The unique identifier of the snapshot being processed.</param>
    /// <param name="runId">The Guid of the rung used for identifying uniqueness.</param>
    /// <param name="rung">The rung to be processed containing instructions and arguments.</param>
    /// <param name="instructionRecords">The collection to which instruction records are added.</param>
    /// <param name="argumentRecords">The collection to which argument records are added.</param>
    private static void ProcessRung(
        int snapshotId,
        Guid runId,
        Rung rung,
        List<InstructionRecord> instructionRecords,
        List<ArgumentRecord> argumentRecords)
    {
        var instructions = rung.Instructions().ToArray();

        for (short index = 0; index < instructions.Length; index++)
        {
            var instruction = instructions[index];
            var instructionRecord = new InstructionRecord(snapshotId, runId, index, instruction);
            var instructionId = instructionRecord.InstructionId;
            instructionRecords.Add(instructionRecord);

            var arguments = instruction.Arguments.ToArray();

            for (byte arg = 0; arg < arguments.Length; arg++)
            {
                var argumentIndex = arg;
                var argument = arguments[argumentIndex];

                if (argument.Type == ArgumentType.Expression)
                {
                    //todo we need a way to get all expression argument including values using L5Sharp.
                    argumentRecords.AddRange(argument.Tags.Select(t =>
                        new ArgumentRecord(snapshotId, instructionId, argumentIndex, new Argument(t))
                    ));
                    continue;
                }

                argumentRecords.Add(new ArgumentRecord(snapshotId, instructionId, argumentIndex, argument));
            }
        }
    }
}