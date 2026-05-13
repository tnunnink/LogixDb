using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

/// <summary>
/// Provides functionality to transform a <see cref="Target"/> object into a collection of
/// <see cref="DataTable"/> instances focused on rungs and their instructions and arguments.
/// </summary>
public class RungTransformer : IDbTransformer
{
    private readonly RungMap _rungMap = new();
    private readonly InstructionMap _instructionMap = new();
    private readonly ArgumentMap _argumentMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource();
        var rungRecords = new List<Rung>();
        var instructionRecords = new List<InstructionRecord>();
        var argumentRecords = new List<ArgumentRecord>();

        var rungs = source.Programs
            .SelectMany(p => p.Routines.Where(r => r.Type == RoutineType.RLL))
            .SelectMany(r => r.Rungs);

        foreach (var rung in rungs)
        {
            // This is for instruction and argument tables to form relational reference.
            var rungId = Guid.CreateVersion7();
            rung.Metadata.Add("rung_id", rungId);

            rungRecords.Add(rung);
            ProcessRung(rungId, rung, instructionRecords, argumentRecords);
        }

        yield return _rungMap.GenerateTable(rungRecords);
        yield return _instructionMap.GenerateTable(instructionRecords);
        yield return _argumentMap.GenerateTable(argumentRecords);
    }

    private static void ProcessRung(Guid? rungKey, Rung rung,
        List<InstructionRecord> instructionRecords,
        List<ArgumentRecord> argumentRecords)
    {
        var instructions = rung.Instructions().ToArray();

        for (short i = 0; i < instructions.Length; i++)
        {
            var instruction = instructions[i];

            instructionRecords.Add(new InstructionRecord(
                rungKey,
                i,
                instruction.ToString(),
                instruction.Key,
                instruction.IsConditional,
                instruction.IsNative
            ));

            var arguments = instruction.Arguments.ToArray();
            for (byte a = 0; a < arguments.Length; a++)
            {
                var instructionIndex = i;
                var argumentIndex = a;
                var argument = arguments[argumentIndex];

                if (argument.Type == ArgumentType.Expression)
                {
                    //todo I feel like this needs some review to determine if we are capturing what we want.
                    var nestedArgs = argument.Tags.Select(t => new Argument(t));
                    argumentRecords.AddRange(nestedArgs.Select(x => new ArgumentRecord(
                        rungKey,
                        instructionIndex,
                        argumentIndex,
                        x.Type,
                        x.ToString())
                    ));
                    continue;
                }

                argumentRecords.Add(new ArgumentRecord(
                    rungKey,
                    instructionIndex,
                    argumentIndex,
                    argument.Type,
                    argument.ToString()
                ));
            }
        }
    }
}