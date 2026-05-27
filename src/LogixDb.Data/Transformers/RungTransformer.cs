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
        var source = target.GetSource();
        var rungRecords = new List<Rung>();
        var instructionRecords = new List<InstructionRecord>();
        var argumentRecords = new List<ArgumentRecord>();
        var referenceRecords = new List<ReferenceRecord>();

        // Should get all program and AOI rnug logic.
        var rungs = source.Query<Routine>().Where(r => r.Type == RoutineType.RLL).SelectMany(r => r.Rungs);

        foreach (var rung in rungs)
        {
            // This is for child tables to form relational reference.
            var rungId = Guid.CreateVersion7();
            rung.Metadata.Add("rung_id", rungId);

            rungRecords.Add(rung);
            ProcessRung(rungId, rung, instructionRecords, argumentRecords, referenceRecords);
        }

        yield return _rungMap.GenerateTable(rungRecords);
        yield return _instructionMap.GenerateTable(instructionRecords);
        yield return _argumentMap.GenerateTable(argumentRecords);
        yield return _referenceMap.GenerateTable(referenceRecords);
    }

    private static void ProcessRung(Guid rungId, Rung rung,
        List<InstructionRecord> instructionRecords,
        List<ArgumentRecord> argumentRecords,
        List<ReferenceRecord> referenceRecords)
    {
        var instructions = rung.Instructions().ToArray();

        for (short i = 0; i < instructions.Length; i++)
        {
            var instruction = instructions[i];

            instructionRecords.Add(new InstructionRecord(
                rungId,
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
                argumentRecords.Add(new ArgumentRecord(rungId, i, a, argument.Type, argument.ToString()));

                foreach (var reference in GetReferences(argument))
                    referenceRecords.Add(new ReferenceRecord(rungId, i, a, reference));
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