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
            .WithPrimaryKey<long>("argument_id")
            .WithRelation<long>("instruction_id", "instruction").NotNullable()
            .WithColumn("argument_index").AsByte().NotNullable()
            .WithColumn("argument_type").AsString(32).NotNullable()
            .WithColumn("argument_text").AsString(256).NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();
        
        Create.Index().OnTable("argument")
            .OnColumn("instruction_id").Ascending()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("argument")
            .OnColumn("instruction_id").Ascending()
            .OnColumn("argument_index").Ascending();

        Create.Index().OnTable("argument")
            .OnColumn("argument_text").Ascending();
    }
}