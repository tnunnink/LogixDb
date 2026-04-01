using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260212;

[UsedImplicitly]
[Migration(202602120900, "Creates aoi_parameter table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Aoi)]
public class M012CreateAoiParameterTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("aoi_parameter")
            .WithPrimaryGuid("parameter_id")
            .WithNumericCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("aoi_name").AsString(128).NotNullable()
            .WithColumn("parameter_name").AsString(128).NotNullable()
            .WithColumn("parameter_description").AsString(512).Nullable()
            .WithColumn("data_type").AsString(128).Nullable()
            .WithColumn("default_value").AsString(128).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("tag_usage").AsString(32).Nullable()
            .WithColumn("tag_type").AsString(32).Nullable()
            .WithColumn("tag_alias").AsString(128).Nullable()
            .WithColumn("is_visible").AsBoolean().Nullable()
            .WithColumn("is_required").AsBoolean().Nullable()
            .WithColumn("is_constant").AsBoolean().Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index()
            .OnTable("aoi_parameter")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("aoi_name").Ascending()
            .OnColumn("parameter_name").Ascending()
            .WithOptions().Unique();
    }
}