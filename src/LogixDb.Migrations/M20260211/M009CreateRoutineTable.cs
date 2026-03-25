using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111930, "Creates routine table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Component, MigrationTag.Routine)]
public class M009CreateRoutineTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("routine")
            .WithPrimaryGuid("routine_id")
            .WithNumericCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("program_name").AsString(128).NotNullable()
            .WithColumn("routine_name").AsString(128).NotNullable()
            .WithColumn("routine_type").AsString(32).Nullable()
            .WithColumn("routine_description").AsString(512).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index()
            .OnTable("routine")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("program_name").Ascending()
            .OnColumn("routine_name").Ascending()
            .WithOptions().Unique();

        Create.Index()
            .OnTable("routine")
            .OnColumn("record_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}