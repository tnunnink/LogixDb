using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260306;

[UsedImplicitly]
[Migration(202603061100, "Creates rung table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202603061100CreateRungTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("rung")
            .WithPrimaryKey<long>("rung_id")
            .WithColumn("container_name").AsString(256).NotNullable()
            .WithColumn("routine_name").AsString(256).NotNullable()
            .WithColumn("rung_number").AsInt32().NotNullable()
            .WithColumn("rung_text").AsString(int.MaxValue).Nullable()
            .WithColumn("rung_comment").AsString(int.MaxValue).Nullable()
            .WithColumn("code_hash").AsString(64).Nullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("rung")
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("rung")
            .OnColumn("container_name").Ascending()
            .OnColumn("routine_name").Ascending()
            .OnColumn("rung_number").Ascending();

        Create.Index().OnTable("rung")
            .OnColumn("routine_name").Ascending()
            .OnColumn("rung_number").Ascending();

        Create.Index().OnTable("rung")
            .OnColumn("code_hash").Ascending();
    }
}