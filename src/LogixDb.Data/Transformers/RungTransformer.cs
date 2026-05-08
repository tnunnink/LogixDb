using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Extensions;
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
        var rungRecords = new List<Rung>();
        var instructionRecords = new List<InstructionRecord>();
        var argumentRecords = new List<ArgumentRecord>();

        var rungs = source.Programs
            .SelectMany(p => p.Routines.Where(r => r.Type == RoutineType.RLL))
            .SelectMany(r => r.Rungs);

        foreach (var rung in rungs)
        {
            var rungHash = rung.Hash();
            rungRecords.Add(rung);
            ProcessRung(rungHash, rung, instructionRecords, argumentRecords);
        }

        yield return _rungMap.GenerateTable(rungRecords);
        yield return _instructionMap.GenerateTable(instructionRecords);
        yield return _argumentMap.GenerateTable(argumentRecords);
    }

    private static void ProcessRung(string? rungHash, Rung rung,
        List<InstructionRecord> instructionRecords,
        List<ArgumentRecord> argumentRecords)
    {
        var instructions = rung.Instructions().ToArray();

        for (short index = 0; index < instructions.Length; index++)
        {
            var instruction = instructions[index];

            var instructionRecord = new InstructionRecord(
                rungHash,
                index,
                instruction.ToString(),
                instruction.Key,
                instruction.IsConditional,
                instruction.IsNative
            );

            instructionRecords.Add(instructionRecord);

            var instructionHash = instructionRecord.Hash(["RungId"]);
            var arguments = instruction.Arguments.ToArray();

            for (byte arg = 0; arg < arguments.Length; arg++)
            {
                var argumentIndex = arg;
                var argument = arguments[argumentIndex];

                if (argument.Type == ArgumentType.Expression)
                {
                    var nestedArgs = argument.Tags
                        .Select(t => new Argument(t))
                        .Select(a => new ArgumentRecord(instructionHash, argumentIndex, a.Type, a.ToString()));

                    argumentRecords.AddRange(nestedArgs);
                    continue;
                }

                argumentRecords.Add(new ArgumentRecord(
                    instructionHash,
                    argumentIndex,
                    argument.Type,
                    argument.ToString()
                ));
            }
        }
    }
}