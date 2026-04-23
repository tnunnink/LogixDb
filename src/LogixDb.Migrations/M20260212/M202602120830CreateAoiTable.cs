using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260212;

[UsedImplicitly]
[Migration(202602120830, "Creates aoi table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Aoi)]
public class M202602120830CreateAoiTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("aoi")
            .WithPrimaryKey("aoi_id")
            .WithRelation("instance_id", "target_instance").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("aoi_name").AsString(256).NotNullable()
            .WithColumn("aoi_description").AsString(512).Nullable()
            .WithColumn("aoi_revision").AsString(16).Nullable()
            .WithColumn("aoi_revision_extension").AsString(64).Nullable()
            .WithColumn("aoi_revision_note").AsString(512).Nullable()
            .WithColumn("aoi_vendor").AsString(64).Nullable()
            .WithColumn("aoi_help_text").AsString(int.MaxValue).Nullable()
            .WithColumn("created_date").AsDateTime().Nullable()
            .WithColumn("created_by").AsString(64).Nullable()
            .WithColumn("edited_date").AsDateTime().Nullable()
            .WithColumn("edited_by").AsString(64).Nullable()
            .WithColumn("software_revision").AsString(16).Nullable()
            .WithColumn("execute_pre_scan").AsBoolean().Nullable()
            .WithColumn("execute_post_scan").AsBoolean().Nullable()
            .WithColumn("execute_enable_in_false").AsBoolean().Nullable()
            .WithColumn("is_encrypted").AsBoolean().Nullable()
            .WithColumn("signature_id").AsString(32).Nullable()
            .WithColumn("signature_timestamp").AsDateTime().Nullable()
            .WithColumn("component_class").AsString(32).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable()
            .WithColumn("source_hash").AsString(32).NotNullable();

        Create.Index().OnTable("aoi")
            .OnColumn("instance_id").Ascending()
            .OnColumn("aoi_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("aoi")
            .OnColumn("aoi_name").Ascending()
            .OnColumn("record_hash").Ascending();
        
        Create.Index().OnTable("aoi")
            .OnColumn("source_hash").Ascending()
            .OnColumn("instance_id").Ascending();
    }
}