using System.Data;
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
        Create.Table("tag_member_comment").InLogixSchema()
            .WithRelation<long>("tag_id", "tag").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("member_path").AsString(256).NotNullable()
            .WithColumn("comment").AsString(int.MaxValue).NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index()
            .OnTable("tag_member_comment").InLogixSchema()
            .OnColumn("tag_id").Ascending()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index()
            .OnTable("tag_member_comment").InLogixSchema()
            .OnColumn("tag_id").Ascending()
            .OnColumn("member_path").Ascending()
            .WithOptions().Unique()
            .WithOptions().Clustered();

        Create.Index()
            .OnTable("tag_member_comment").InLogixSchema()
            .OnColumn("member_path").Ascending();
    }
}