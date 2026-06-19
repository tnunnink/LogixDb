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
            .Create.Table("tag_member").InLogixSchema()
            .WithPrimaryKey<long>("member_id")
            .WithRelation<long>("tag_id", "tag").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("member_path").AsString(256).NotNullable()
            .WithColumn("member_name").AsString(128).Nullable()
            .WithColumn("parent_path").AsString(128).Nullable()
            .WithColumn("data_type").AsString(128).NotNullable();

        // SQL server supports creating clustered index on non PK columns which we want for performance.
        // This is the only syntax that works for both providers...
        IfDatabase(ProcessorIdConstants.SqlServer)
            .Create.Table("tag_member").InLogixSchema()
            .WithColumn("member_id").AsInt64().NotNullable().Identity()
            .WithRelation<long>("tag_id", "tag").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("member_path").AsString(256).NotNullable()
            .WithColumn("parent_path").AsString(128).Nullable()
            .WithColumn("member_name").AsString(128).Nullable()
            .WithColumn("data_type").AsString(128).NotNullable();

        IfDatabase(ProcessorIdConstants.SqlServer)
            .Create.PrimaryKey()
            .OnTable("tag_member")
            .Column("member_id")
            .NonClustered();

        IfDatabase(ProcessorIdConstants.SqlServer)
            .Create.Index()
            .OnTable("tag_member").InLogixSchema()
            .OnColumn("tag_id").Ascending()
            .OnColumn("member_path").Ascending()
            .WithOptions().Unique()
            .WithOptions().Clustered();

        IfDatabase(ProcessorIdConstants.SQLite)
            .Create.Index()
            .OnTable("tag_member").InLogixSchema()
            .OnColumn("tag_id").Ascending()
            .OnColumn("member_path").Ascending()
            .WithOptions().Unique();

        Create.Index()
            .OnTable("tag_member").InLogixSchema()
            .OnColumn("member_path").Ascending();

        Create.Index()
            .OnTable("tag_member").InLogixSchema()
            .OnColumn("parent_path").Ascending()
            .OnColumn("tag_id").Ascending();

        Create.Index()
            .OnTable("tag_member").InLogixSchema()
            .OnColumn("member_name").Ascending()
            .OnColumn("tag_id").Ascending();

        Create.Index()
            .OnTable("tag_member").InLogixSchema()
            .OnColumn("data_type").Ascending()
            .OnColumn("tag_id").Ascending();
    }
}