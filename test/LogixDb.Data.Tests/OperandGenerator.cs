using System.Reflection;
using System.Text;
using L5Sharp.Core;

namespace LogixDb.Data.Tests;

[TestFixture]
public class OperandGenerator
{
    [Test]
    public void GetInstructionArgumentsTableDataForSeeding()
    {
        var type = typeof(Instruction);
        var builder = new StringBuilder();

        var factories = type
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.ReturnType == typeof(Instruction) && m.Name.All(char.IsUpper));

        foreach (var factory in factories)
        {
            var arguments = factory.GetParameters()
                .Select((p, i) => new { p.Name, Index = i })
                .ToList();

            arguments.ForEach(a =>
            {
                var isDestructive = InferDestructive(factory.Name, a.Name);

                builder.AppendLine(
                    $$"""
                      insert.Row(new
                      {
                          snapshot_id = 0,
                          instruction_key = "{{factory.Name}}",
                          operand_index = {{a.Index}}, 
                          operand_name = "{{a.Name}}", 
                          is_destructive = {{isDestructive.ToString().ToLower()}}
                      });

                      """);
            });
        }

        File.WriteAllText(@"c:\users\tnunnink\desktop\instructions.csv", builder.ToString());
    }

    private static bool InferDestructive(string key, string? arg)
    {
        if (string.IsNullOrEmpty(arg)) return false;
        if (key is "OTE" or "OTU" or "OTL" && arg == "data_bit") return true;
        if (string.Equals(arg, "destination", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(arg, "storage_bit", StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(arg, "output_bit", StringComparison.OrdinalIgnoreCase)) return true;
        if (arg.Contains("control", StringComparison.OrdinalIgnoreCase)) return true;
        if (arg.EndsWith("_tag", StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }
}