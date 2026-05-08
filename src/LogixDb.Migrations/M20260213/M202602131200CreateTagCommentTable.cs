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
            .WithPrimaryKey<long>("comment_id")
            .WithRelation<long>("member_id", "tag_member").NotNullable()
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("tag_comment").AsString(int.MaxValue).NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("tag_comment")
            .OnColumn("member_id").Ascending()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_comment")
            .OnColumn("member_id").Ascending()
            .OnColumn("tag_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_comment")
            .OnColumn("tag_name").Ascending()
            .OnColumn("member_id").Ascending();
    }
}