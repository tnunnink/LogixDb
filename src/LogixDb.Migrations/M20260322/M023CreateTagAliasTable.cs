using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260322;

[UsedImplicitly]
[Migration(202603220600, "Creates tag_alias table with corresponding indexes")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M023CreateTagAliasTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_alias")
            .WithPrimaryGuid("alias_id")
            .WithNumericCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("tag_id").AsGuid().NotNullable()
            .WithColumn("alias_for").AsString(256).NotNullable();

        Create.Index().OnTable("tag_alias")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("tag_id").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_alias")
            .OnColumn("alias_for").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}