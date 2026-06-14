using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Abstractions;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Transformers;

public class RungTransformer : IDbTransformer
{
    private readonly RungMap _rungMap = new();
    private readonly RungInstructionMap _instructionMap = new();
    private readonly RungArgumentMap _argumentMap = new();
    private readonly RungReferenceMap _referenceMap = new();

    /// <inheritdoc />
    public IEnumerable<DataTable> Transform(Target target)
    {
        var source = target.GetSource(scrub: true);
        var rungRecords = new List<Rung>();
        var instructionRecords = new List<InstructionRecord>();
        var argumentRecords = new List<ArgumentRecord>();
        var referenceRecords = new List<ReferenceRecord>();

        // Should get all program and AOI rung logic.
        var rungs = source.Query<Routine>().Where(r => r.Type == RoutineType.RLL).SelectMany(r => r.Rungs);

        foreach (var rung in rungs)
        {
            // This forces the hash for the parent to be computed and cached so we can pass to child records.
            var rungHash = _rungMap.ComputeHash(rung);
            rungRecords.Add(rung);
            ProcessRung(rungHash, rung, instructionRecords, argumentRecords, referenceRecords);
        }

        yield return _rungMap.GenerateTable(rungRecords);
        yield return _instructionMap.GenerateTable(instructionRecords);
        yield return _argumentMap.GenerateTable(argumentRecords);
        yield return _referenceMap.GenerateTable(referenceRecords);
    }

    private static void ProcessRung(string rungHash, Rung rung,
        List<InstructionRecord> instructionRecords,
        List<ArgumentRecord> argumentRecords,
        List<ReferenceRecord> referenceRecords)
    {
        var instructions = rung.Instructions().ToArray();

        for (short i = 0; i < instructions.Length; i++)
        {
            var instruction = instructions[i];

            instructionRecords.Add(new InstructionRecord(
                rungHash,
                i,
                instruction.ToString(),
                instruction.Key,
                instruction.IsConditional,
                instruction.IsNative
            ));

            var arguments = instruction.Arguments.ToArray();

            for (byte a = 0; a < arguments.Length; a++)
            {
                var argument = arguments[a];
                argumentRecords.Add(new ArgumentRecord(rungHash, i, a, argument.Type, argument.ToString()));

                foreach (var reference in GetReferences(argument))
                    referenceRecords.Add(new ReferenceRecord(rungHash, i, a, reference));
            }
        }
    }

    /// <summary>
    /// This is a simple helper to get "reference" tag names from the argument. For most cases this just returns the tag
    /// name if this is a reference argument. For expressions, we want to extract all nested tag names for reference lookup.
    /// </summary>
    private static IEnumerable<TagName> GetReferences(Argument argument)
    {
        if (argument.IsReference)
            return [argument.ToTagName()];

        if (argument.IsExpression)
            return TagName.Parse(argument.ToNeutralText());

        return [];
    }
}