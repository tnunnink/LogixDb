using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111930, "Creates routine table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202602111930CreateRoutineTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("routine")
            .WithPrimaryGuid("routine_id")
            .WithInstanceRelation()
            .WithParentRelation("program_id", "program")
            .WithColumn("routine_name").AsString(256).NotNullable()
            .WithColumn("routine_description").AsString(512).Nullable()
            .WithColumn("routine_type").AsString(32).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable()
            .WithColumn("source_hash").AsString(32).NotNullable();

        Create.Index().OnTable("routine")
            .OnColumn("program_id").Ascending()
            .OnColumn("routine_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("routine")
            .OnColumn("routine_name").Ascending()
            .OnColumn("record_hash").Ascending();
        
        Create.Index().OnTable("routine")
            .OnColumn("source_hash").Ascending()
            .OnColumn("program_id").Ascending();
    }
}