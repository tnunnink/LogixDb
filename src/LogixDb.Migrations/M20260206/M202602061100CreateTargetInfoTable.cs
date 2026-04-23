using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260206;

[UsedImplicitly]
[Migration(202602061100, "Create target_info table for target version metadata")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202602061100CreateTargetInfoTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("target_info")
            .WithPrimaryKey("property_id")
            .WithRelation("version_id", "target_version").OnDeleteOrUpdate(Rule.Cascade).NotNullable()
            .WithColumn("property_name").AsString().NotNullable()
            .WithColumn("property_value").AsString().Nullable();

        Create.Index()
            .OnTable("target_info")
            .OnColumn("version_id").Ascending()
            .OnColumn("property_name").Ascending()
            .WithOptions().Unique();
    }
}