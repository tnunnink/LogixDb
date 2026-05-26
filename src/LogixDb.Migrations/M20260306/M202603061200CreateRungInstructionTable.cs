using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260306;

[UsedImplicitly]
[Migration(202603061200, "Creates instruction table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202603061200CreateRungInstructionTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("rung_instruction")
            .WithRelation<Guid>("rung_id", "rung").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("instruction_index").AsInt16().NotNullable()
            .WithColumn("instruction_text").AsString(int.MaxValue).NotNullable()
            .WithColumn("instruction_key").AsString(128).NotNullable()
            .WithColumn("is_conditional").AsBoolean().NotNullable()
            .WithColumn("is_native").AsBoolean().NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("rung_instruction")
            .OnColumn("rung_id").Ascending()
            .OnColumn("instruction_index").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("rung_instruction")
            .OnColumn("instruction_key").Ascending();
        
        Create.Index().OnTable("rung_instruction")
            .OnColumn("record_hash").Ascending();
    }
}