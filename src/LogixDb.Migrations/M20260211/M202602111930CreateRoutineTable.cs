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
            .WithPrimaryKey<long>("routine_id")
            .WithColumn("container_name").AsString(256).NotNullable()
            .WithColumn("routine_name").AsString(256).NotNullable()
            .WithColumn("routine_description").AsString(512).Nullable()
            .WithColumn("routine_type").AsString(32).Nullable()
            .WithColumn("is_definition").AsBoolean().NotNullable()
            .WithColumn("content_hash").AsString(64).NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("routine")
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("routine")
            .OnColumn("container_name").Ascending()
            .OnColumn("routine_name").Ascending();

        Create.Index().OnTable("routine")
            .OnColumn("routine_name").Ascending();
    }
}