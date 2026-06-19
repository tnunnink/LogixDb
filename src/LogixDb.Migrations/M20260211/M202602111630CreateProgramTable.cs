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
        Create.Table("program").InLogixSchema()
            .WithPrimaryKey<long>("program_id")
            .WithColumn("program_name").AsString(256).NotNullable()
            .WithColumn("task_name").AsString(256).Nullable()
            .WithColumn("folder_name").AsString(256).Nullable()
            .WithColumn("program_description").AsString(512).Nullable()
            .WithColumn("program_type").AsString(32).Nullable()
            .WithColumn("main_routine").AsString(64).Nullable()
            .WithColumn("fault_routine").AsString(64).Nullable()
            .WithColumn("is_disabled").AsBoolean().Nullable()
            .WithColumn("is_folder").AsBoolean().Nullable()
            .WithColumn("has_test_edits").AsBoolean().Nullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("program").InLogixSchema()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("program").InLogixSchema()
            .OnColumn("program_name").Ascending();

        Create.Index().OnTable("program").InLogixSchema()
            .OnColumn("folder_name").Ascending();

        Create.Index().OnTable("program").InLogixSchema()
            .OnColumn("task_name").Ascending();
    }
}