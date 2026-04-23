using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260306;

[UsedImplicitly]
[Migration(202603061200, "Creates instruction table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202603061200CreateInstructionTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("instruction")
            .WithPrimaryKey("instruction_id")
            .WithRelation<int>("instance_id", "target_instance").OnDelete(Rule.Cascade).NotNullable()
            .WithRelation<Guid>("rung_id", "rung").NotNullable()
            .WithColumn("instruction_index").AsInt16().NotNullable()
            .WithColumn("instruction_text").AsString(int.MaxValue).NotNullable()
            .WithColumn("instruction_key").AsString(128).NotNullable()
            .WithColumn("is_conditional").AsBoolean().NotNullable()
            .WithColumn("is_native").AsBoolean().NotNullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("instruction")
            .OnColumn("rung_id").Ascending()
            .OnColumn("instruction_index").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("instruction")
            .OnColumn("record_hash").Ascending()
            .OnColumn("rung_id").Ascending();
    }
}