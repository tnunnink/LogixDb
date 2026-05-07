using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111630, "Creates program table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag, MigrationTag.Logic)]
public class M202602111630CreateProgramTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("program")
            .WithPrimaryKey("program_id")
            .WithRelation<Guid>("task_id", "task").Nullable()
            .WithRelation<Guid>("folder_id", "program", "program_id").Nullable()
            .WithColumn("program_name").AsString(256).NotNullable()
            .WithColumn("program_description").AsString(512).Nullable()
            .WithColumn("program_type").AsString(32).Nullable()
            .WithColumn("main_routine").AsString(64).Nullable()
            .WithColumn("fault_routine").AsString(64).Nullable()
            .WithColumn("is_disabled").AsBoolean().Nullable()
            .WithColumn("is_folder").AsBoolean().Nullable()
            .WithColumn("has_test_edits").AsBoolean().Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable()
            .WithColumn("source_hash").AsString(32).NotNullable();
        
        Create.Index().OnTable("program")
            .OnColumn("task_id").Ascending()
            .OnColumn("folder_id").Ascending()
            .OnColumn("source_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("program")
            .OnColumn("folder_id").Ascending()
            .OnColumn("program_id").Ascending();

        Create.Index().OnTable("program")
            .OnColumn("program_name").Ascending()
            .OnColumn("record_hash").Ascending();

        Create.Index().OnTable("program")
            .OnColumn("program_name").Ascending()
            .OnColumn("source_hash").Ascending();
    }
}