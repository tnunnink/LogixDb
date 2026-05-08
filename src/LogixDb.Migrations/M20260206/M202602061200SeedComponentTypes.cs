using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260206;

[UsedImplicitly]
[Migration(202602061200, "Seeds component table with initial component type records (controller, data_type, aoi, module, tag, program, routine, task)")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202602061200SeedComponentTypes : AutoReversingMigration
{
    public override void Up()
    {
        Insert.IntoTable("component")
            .Row(new { component_id = 1, component_name = "controller" })
            .Row(new { component_id = 2, component_name = "data_type" })
            .Row(new { component_id = 3, component_name = "aoi" })
            .Row(new { component_id = 4, component_name = "module" })
            .Row(new { component_id = 5, component_name = "tag" })
            .Row(new { component_id = 6, component_name = "task" })
            .Row(new { component_id = 7, component_name = "program" })
            .Row(new { component_id = 8, component_name = "routine" })
            .Row(new { component_id = 9, component_name = "rung" });
    }
}