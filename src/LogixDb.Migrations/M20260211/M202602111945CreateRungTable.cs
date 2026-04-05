using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111945, "Creates rung table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Rung)]
public class M202602111945CreateRungTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("rung")
            .WithPrimaryGuid("rung_id")
            .WithRequiredRelation("routine_id", "routine")
            .WithColumn("rung_number").AsInt32().NotNullable()
            .WithColumn("rung_text").AsString(int.MaxValue).Nullable()
            .WithColumn("rung_comment").AsString(int.MaxValue).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index()
            .OnTable("rung")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("routine_id").Ascending()
            .OnColumn("rung_number").Ascending()
            .WithOptions().Unique();

        Create.Index()
            .OnTable("rung")
            .OnColumn("record_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}