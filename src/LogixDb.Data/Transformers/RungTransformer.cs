using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on rungs and their instructions and arguments.
/// </summary>
internal class RungTransformer : IDbTransformer
{
    private readonly RungMap _rungMap = new();
    private readonly InstructionMap _instructionMap = new();
    private readonly ArgumentMap _argumentMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var rungRecords = new List<RungRecord>();
        var instructionRecords = new List<InstructionRecord>();
        var argumentRecords = new List<ArgumentRecord>();

        var rungs = source.Programs
            .SelectMany(p => p.Routines.Where(r => r.Type == RoutineType.RLL))
            .SelectMany(r => r.Rungs);

        foreach (var rung in rungs)
        {
            var routineId = rung.Routine?.Metadata.Get<Guid>("id");
            var rungRecord = new RungRecord(target.InstanceId, routineId, rung);
            rungRecords.Add(rungRecord);
            ProcessRung(rungRecord.RungId, target.InstanceId, rung, instructionRecords, argumentRecords);
        }

        yield return _rungMap.GenerateTable(rungRecords);
        yield return _instructionMap.GenerateTable(instructionRecords);
        yield return _argumentMap.GenerateTable(argumentRecords);
    }

    private static void ProcessRung(Guid rungId, int instanceId, Rung rung,
        List<InstructionRecord> instructionRecords,
        List<ArgumentRecord> argumentRecords)
    {
        var instructions = rung.Instructions().ToArray();

        for (short index = 0; index < instructions.Length; index++)
        {
            var instruction = instructions[index];
            var instructionRecord = new InstructionRecord(instanceId, rungId, index, instruction);
            var instructionId = instructionRecord.InstructionId;
            instructionRecords.Add(instructionRecord);

            var arguments = instruction.Arguments.ToArray();

            for (byte arg = 0; arg < arguments.Length; arg++)
            {
                var argumentIndex = arg;
                var argument = arguments[argumentIndex];

                if (argument.Type == ArgumentType.Expression)
                {
                    argumentRecords.AddRange(argument.Tags.Select(t =>
                        new ArgumentRecord(instanceId, instructionId, argumentIndex, new Argument(t))
                    ));
                    continue;
                }

                argumentRecords.Add(new ArgumentRecord(instanceId, instructionId, argumentIndex, argument));
            }
        }
    }
}
