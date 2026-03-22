using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260311;

[UsedImplicitly]
[Migration(202603110830, "Creates tag_comment table with corresponding indexes and keys")]
public class M019CreateTagCommentTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_comment")
            .WithPrimaryGuid("comment_id")
            .WithNumericCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("tag_id").AsGuid().NotNullable()
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("tag_comment").AsString(int.MaxValue).NotNullable();

        Create.Index()
            .OnTable("tag_comment")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("tag_id").Ascending()
            .OnColumn("tag_name").Ascending();

        Create.Index()
            .OnTable("tag_comment")
            .OnColumn("tag_name").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}