using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111945, "Creates rung table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202602111945CreateRungTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("rung")
            .WithPrimaryKey<long>("rung_id")
            .WithRelation<long>("routine_id", "routine").NotNullable()
            .WithColumn("rung_number").AsInt32().NotNullable()
            .WithColumn("rung_text").AsString(int.MaxValue).Nullable()
            .WithColumn("rung_comment").AsString(int.MaxValue).Nullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("rung")
            .OnColumn("routine_id").Ascending()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("rung")
            .OnColumn("routine_id").Ascending()
            .OnColumn("rung_number").Ascending()
            .WithOptions().Unique();
    }
}