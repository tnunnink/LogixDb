using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260306;

[UsedImplicitly]
[Migration(202603061300, "Creates argument table with corresponding indexes and foreign key relationships")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202603061300CreateArgumentTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("argument")
            .WithPrimaryKey("argument_id")
            .WithRelation("instance_id", "target_instance").OnDelete(Rule.Cascade).NotNullable()
            .WithRelation("instruction_id", "instruction").NotNullable()
            .WithColumn("argument_index").AsByte().NotNullable()
            .WithColumn("argument_type").AsString(32).NotNullable()
            .WithColumn("argument_text").AsString(255).NotNullable();

        Create.Index().OnTable("argument")
            .OnColumn("instruction_id").Ascending()
            .OnColumn("argument_index").Ascending();

        Create.Index().OnTable("argument")
            .OnColumn("argument_text").Ascending();
    }
}