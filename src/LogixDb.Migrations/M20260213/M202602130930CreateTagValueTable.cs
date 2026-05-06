using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260213;

[UsedImplicitly]
[Migration(202602130900, "Creates tag_member table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202602130930CreateTagValueTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_value")
            .WithRelation<int>("version_id", "target_version").OnDelete(Rule.SetNull).Nullable()
            .WithRelation<Guid>("member_id", "tag_member").OnDelete(Rule.Cascade)
            .WithColumn("tag_value").AsString(256).Nullable();
    }
}