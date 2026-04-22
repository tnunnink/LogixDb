using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260213;

[UsedImplicitly]
[Migration(202602131200, "Creates tag_comment table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202602131200CreateTagCommentTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_comment")
            .WithPrimaryGuid("comment_id")
            .WithInstanceRelation()
            .WithParentRelation("member_id", "tag_member")
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("tag_comment").AsString(int.MaxValue).NotNullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("tag_comment")
            .OnColumn("member_id").Ascending()
            .OnColumn("tag_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_comment")
            .OnColumn("tag_name").Ascending()
            .OnColumn("member_id").Ascending();

        Create.Index().OnTable("tag_comment")
            .OnColumn("record_hash").Ascending()
            .OnColumn("tag_name").Ascending();
    }
}