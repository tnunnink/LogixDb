using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260212;

[UsedImplicitly]
[Migration(202602120910, "Creates aoi_rung table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Aoi)]
public class M202602120910CreateAoiRungTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("aoi_rung")
            .WithPrimaryGuid("rung_id")
            .WithRequiredRelation("aoi_id", "aoi")
            .WithColumn("routine_name").AsString(128).NotNullable()
            .WithColumn("rung_number").AsInt32().NotNullable()
            .WithColumn("rung_text").AsString(int.MaxValue).NotNullable()
            .WithColumn("rung_comment").AsString(int.MaxValue).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("aoi_rung")
            .OnColumn("aoi_id").Ascending()
            .OnColumn("routine_name").Ascending()
            .OnColumn("rung_number").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("aoi_rung")
            .OnColumn("record_hash").Ascending()
            .OnColumn("aoi_id").Ascending();
    }
}