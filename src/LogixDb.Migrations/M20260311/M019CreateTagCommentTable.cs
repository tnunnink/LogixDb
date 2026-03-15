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
            .WithPrimaryId("comment_id")
            .WithCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("program_name").AsString(32).NotNullable()
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("tag_comment").AsString(int.MaxValue).NotNullable();

        Create.Index()
            .OnTable("tag_comment")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("program_name").Ascending()
            .OnColumn("tag_name").Ascending();

        Create.Index()
            .OnTable("tag_comment")
            .OnColumn("tag_name").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}