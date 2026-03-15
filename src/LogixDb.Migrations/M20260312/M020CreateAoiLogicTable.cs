using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260312;

[UsedImplicitly]
[Migration(202603122030, "Creates aoi_rung table with corresponding indexes and keys")]
public class M020CreateAoiLogicTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("aoi_rung")
            .WithPrimaryId("rung_id")
            .WithCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("aoi_name").AsString(128).NotNullable()
            .WithColumn("routine_name").AsString(128).NotNullable()
            .WithColumn("rung_number").AsInt32().NotNullable()
            .WithColumn("rung_text").AsString(int.MaxValue).NotNullable()
            .WithColumn("rung_comment").AsString(int.MaxValue).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("aoi_rung")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("aoi_name").Ascending()
            .OnColumn("routine_name").Ascending()
            .OnColumn("rung_number").Ascending()
            .WithOptions().Unique();
        
        Create.Index().OnTable("aoi_rung")
            .OnColumn("record_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}