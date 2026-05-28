using System.Data;
using FluentMigrator;
using FluentMigrator.SqlServer;
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
        IfDatabase(ProcessorIdConstants.SQLite)
            .Create.Table("tag_member")
            .WithPrimaryKey<long>("member_id")
            .WithRelation<long>("tag_id", "tag").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("parent_name").AsString(128).Nullable()
            .WithColumn("member_name").AsString(128).Nullable()
            .WithColumn("data_type").AsString(128).NotNullable();

        // SQL server supports creating clustered index on non PK columns which we want for performance.
        // This is the only syntax that works for both providers...
        IfDatabase(ProcessorIdConstants.SqlServer)
            .Create.Table("tag_member")
            .WithColumn("member_id").AsInt64().NotNullable().Identity()
            .WithRelation<long>("tag_id", "tag").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("parent_name").AsString(128).Nullable()
            .WithColumn("member_name").AsString(128).Nullable()
            .WithColumn("data_type").AsString(128).NotNullable();

        IfDatabase(ProcessorIdConstants.SqlServer)
            .Create.PrimaryKey()
            .OnTable("tag_member")
            .Column("member_id")
            .NonClustered();

        IfDatabase(ProcessorIdConstants.SqlServer)
            .Create.Index()
            .OnTable("tag_member")
            .OnColumn("tag_id").Ascending()
            .OnColumn("tag_name").Ascending()
            .WithOptions().Unique()
            .WithOptions().Clustered();

        IfDatabase(ProcessorIdConstants.SQLite)
            .Create.Index()
            .OnTable("tag_member")
            .OnColumn("tag_id").Ascending()
            .OnColumn("tag_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_member")
            .OnColumn("tag_name").Ascending();

        Create.Index().OnTable("tag_member")
            .OnColumn("parent_name").Ascending()
            .OnColumn("tag_id").Ascending();

        Create.Index().OnTable("tag_member")
            .OnColumn("member_name").Ascending()
            .OnColumn("tag_id").Ascending();

        Create.Index().OnTable("tag_member")
            .OnColumn("data_type").Ascending()
            .OnColumn("tag_id").Ascending();
    }
}