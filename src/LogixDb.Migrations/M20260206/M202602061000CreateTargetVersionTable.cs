using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260206;

[UsedImplicitly]
[Migration(202602061000, "Create target_version table and associated indexes for target type/name and import date")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202602061000CreateTargetVersionTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("target_version")
            .WithPrimaryKey("version_id")
            .WithRelation("target_id", "target").OnDeleteOrUpdate(Rule.Cascade).NotNullable()
            .WithColumn("version_number").AsInt32().NotNullable()
            .WithColumn("target_type").AsString(128).NotNullable()
            .WithColumn("target_name").AsString(128).NotNullable()
            .WithColumn("schema_revision").AsString(16).Nullable()
            .WithColumn("software_revision").AsString(16).Nullable()
            .WithColumn("is_partial").AsBoolean().NotNullable()
            .WithColumn("export_date").AsDateTime().Nullable()
            .WithColumn("export_options").AsString(256).Nullable()
            .WithColumn("import_date").AsDateTime().NotNullable()
            .WithColumn("import_user").AsString(64).NotNullable()
            .WithColumn("import_machine").AsString(64).NotNullable()
            .WithColumn("source_hash").AsString(32).NotNullable()
            .WithColumn("source_data").AsBinary(int.MaxValue).NotNullable();

        Create.Index()
            .OnTable("target_version")
            .OnColumn("target_type").Ascending()
            .OnColumn("target_name").Ascending();

        Create.Index()
            .OnTable("target_version")
            .OnColumn("target_id").Ascending()
            .OnColumn("version_number").Ascending()
            .WithOptions().Unique();

        Create.Index()
            .OnTable("target_version")
            .OnColumn("target_id").Ascending()
            .OnColumn("import_date").Descending();
    }
}