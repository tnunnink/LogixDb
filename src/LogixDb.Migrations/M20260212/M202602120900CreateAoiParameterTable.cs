using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260212;

[UsedImplicitly]
[Migration(202602120900, "Creates aoi_parameter table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Aoi)]
public class M202602120900CreateAoiParameterTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("aoi_parameter")
            .WithPrimaryKey("parameter_id")
            .WithRelation<int>("instance_id", "target_instance").OnDelete(Rule.Cascade).NotNullable()
            .WithRelation<Guid>("aoi_id", "aoi").NotNullable()
            .WithColumn("parameter_name").AsString(256).NotNullable()
            .WithColumn("parameter_description").AsString(512).Nullable()
            .WithColumn("data_type").AsString(256).Nullable()
            .WithColumn("dimensions").AsString(32).Nullable()
            .WithColumn("radix").AsString(32).Nullable()
            .WithColumn("default_value").AsString(256).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("tag_usage").AsString(32).Nullable()
            .WithColumn("tag_type").AsString(32).Nullable()
            .WithColumn("tag_alias").AsString(128).Nullable()
            .WithColumn("is_visible").AsBoolean().Nullable()
            .WithColumn("is_required").AsBoolean().Nullable()
            .WithColumn("is_constant").AsBoolean().Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("aoi_parameter")
            .OnColumn("aoi_id").Ascending()
            .OnColumn("parameter_name").Ascending()
            .WithOptions().Unique();
        
        Create.Index().OnTable("aoi_parameter")
            .OnColumn("parameter_name").Ascending()
            .OnColumn("record_hash").Ascending();
    }
}