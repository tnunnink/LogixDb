using System.Data;
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
            .WithPrimaryKey<Guid>("rung_id")
            .WithRelation<long>("routine_id", "routine").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("rung_number").AsInt32().NotNullable()
            .WithColumn("rung_text").AsString(int.MaxValue).Nullable()
            .WithColumn("rung_comment").AsString(int.MaxValue).Nullable()
            .WithColumn("code_hash").AsString(64).Nullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("rung")
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("rung")
            .OnColumn("routine_id").Ascending()
            .OnColumn("rung_number").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("rung")
            .OnColumn("code_hash").Ascending();
    }
}