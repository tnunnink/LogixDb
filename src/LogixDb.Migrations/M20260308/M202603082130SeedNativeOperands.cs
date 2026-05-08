using System.Reflection;
using System.Text.Json;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;
using LogixDb.Data.Extensions;
using LogixDb.Data.Maps;

// ReSharper disable StringLiteralTypo

namespace LogixDb.Migrations.M20260308;

[UsedImplicitly]
[Migration(202603082130, "Seeds native operand metadata for all supported Logix instructions")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic, MigrationTag.Aoi)]
public class M202603082130SeedNativeOperands : AutoReversingMigration
{
    public override void Up()
    {
        var operands = LoadResource("operands.json");
        var records = JsonSerializer.Deserialize<List<OperandRecord>>(operands);
        if (records is null) return;

        var insert = Insert.IntoTable("operand");

        foreach (var record in records)
        {
            insert.Row(new
            {
                instruction_key = record.Key,
                operand_index = record.Index,
                operand_name = record.Name,
                operand_type = record.Type,
                operand_description = record.Description,
                is_destructive = record.Destructive,
                record_hash = record.Hash()
            });
        }
    }

    private static string LoadResource(string resource)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"LogixDb.Migrations.Resources.{resource}";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new FileNotFoundException($"'{resource}' not found as embedded resource.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}