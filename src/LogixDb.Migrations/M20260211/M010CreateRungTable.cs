using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111945, "Creates rung table with corresponding indexes and keys")]
public class M010CreateRungTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("rung")
            .WithPrimaryId("rung_id")
            .WithCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("rung_number").AsInt32().NotNullable()
            .WithColumn("routine_name").AsString(128).NotNullable()
            .WithColumn("comment").AsString(int.MaxValue).Nullable()
            .WithColumn("code").AsString(int.MaxValue).Nullable()
            .WithColumn("code_hash").AsString(32).NotNullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index()
            .OnTable("rung")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("routine_name").Ascending()
            .OnColumn("rung_number").Ascending();

        Create.Index()
            .OnTable("rung")
            .OnColumn("record_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();

        Create.Index()
            .OnTable("rung")
            .OnColumn("code_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}