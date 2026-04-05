using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111930, "Creates routine table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Routine)]
public class M202602111930CreateRoutineTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("routine")
            .WithPrimaryGuid("routine_id")
            .WithRequiredRelation("program_id", "program")
            .WithColumn("routine_name").AsString(256).NotNullable()
            .WithColumn("routine_description").AsString(512).Nullable()
            .WithColumn("routine_type").AsString(32).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("routine")
            .OnColumn("program_id").Ascending()
            .OnColumn("routine_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("routine")
            .OnColumn("record_hash").Ascending();
    }
}