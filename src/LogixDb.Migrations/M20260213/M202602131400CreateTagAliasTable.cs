using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260213;

[UsedImplicitly]
[Migration(202602131400, "Creates tag_alias table with corresponding indexes")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202602131400CreateTagAliasTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_alias")
            .WithPrimaryGuid("alias_id")
            .WithRequiredRelation("tag_id", "tag")
            .WithColumn("alias_for").AsString(256).NotNullable();

        Create.Index().OnTable("tag_alias")
            .OnColumn("tag_id").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_alias")
            .OnColumn("alias_for").Ascending();
    }
}