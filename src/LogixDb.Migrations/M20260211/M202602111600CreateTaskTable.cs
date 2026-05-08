using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111600, "Creates task table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag, MigrationTag.Logic)]
public class M202602111600CreateTaskTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("task")
            .WithPrimaryKey<long>("task_id")
            .WithColumn("task_name").AsString(256).NotNullable()
            .WithColumn("task_description").AsString(512).Nullable()
            .WithColumn("task_type").AsString(32).Nullable()
            .WithColumn("priority").AsByte().Nullable()
            .WithColumn("scan_rate").AsFloat().Nullable()
            .WithColumn("watchdog").AsFloat().Nullable()
            .WithColumn("is_inhibited").AsBoolean().Nullable()
            .WithColumn("disable_outputs").AsBoolean().Nullable()
            .WithColumn("event_trigger").AsString(32).Nullable()
            .WithColumn("event_tag").AsString(128).Nullable()
            .WithColumn("enable_timeout").AsBoolean().Nullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("task")
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("task")
            .OnColumn("task_name").Ascending();
    }
}