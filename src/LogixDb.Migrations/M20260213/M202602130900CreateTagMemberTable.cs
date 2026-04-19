using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260213;

[UsedImplicitly]
[Migration(202602130900, "Creates tag_member table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202602130900CreateTagMemberTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_member")
            .WithPrimaryGuid("member_id")
            .WithSnapshotRelation()
            .WithParentRelation("tag_id", "tag")
            .WithParentRelation("parent_id", "tag_member", "member_id", nullable: true)
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("member_name").AsString(128).NotNullable()
            .WithColumn("data_type").AsString(128).Nullable()
            .WithColumn("tag_value").AsString(256).Nullable();

        Create.Index().OnTable("tag_member")
            .OnColumn("tag_id").Ascending()
            .OnColumn("tag_name").Ascending()
            .WithOptions().Unique();
        
        Create.Index().OnTable("tag_member")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("tag_name").Ascending();

        Create.Index().OnTable("tag_member")
            .OnColumn("parent_id").Ascending()
            .OnColumn("member_name").Ascending();

        Create.Index().OnTable("tag_member")
            .OnColumn("tag_name").Ascending()
            .OnColumn("tag_id").Ascending();

        Create.Index().OnTable("tag_member")
            .OnColumn("data_type").Ascending()
            .OnColumn("tag_id").Ascending();
    }
}