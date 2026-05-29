using System.Data;
using FluentMigrator;
using FluentMigrator.SqlServer;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111530, "Creates data_type_member table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.DataType)]
public class M202602111530CreateDataTypeMemberTable : AutoReversingMigration
{
    public override void Up()
    {
        IfDatabase(ProcessorIdConstants.SQLite)
            .Create.Table("data_type_member")
            .WithPrimaryKey<long>("member_id")
            .WithRelation<long>("type_id", "data_type").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("member_name").AsString(256).NotNullable()
            .WithColumn("member_description").AsString(512).Nullable()
            .WithColumn("member_index").AsInt32().NotNullable()
            .WithColumn("data_type").AsString(256).Nullable()
            .WithColumn("dimensions").AsString(32).Nullable()
            .WithColumn("radix").AsString(32).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("is_hidden").AsBoolean().Nullable()
            .WithColumn("target_name").AsString(64).Nullable()
            .WithColumn("bit_number").AsByte().Nullable()
            .WithColumn("record_hash").AsString(64).NotNullable();
        
        // SQL server supports creating clustered index on non PK columns which we want for performance.
        // This is the only syntax that works for both providers...
        IfDatabase(ProcessorIdConstants.SqlServer)
            .Create.Table("data_type_member")
            .WithColumn("member_id").AsInt64().NotNullable().Identity()
            .WithRelation<long>("type_id", "data_type").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("member_name").AsString(256).NotNullable()
            .WithColumn("member_description").AsString(512).Nullable()
            .WithColumn("member_index").AsInt32().NotNullable()
            .WithColumn("data_type").AsString(256).Nullable()
            .WithColumn("dimensions").AsString(32).Nullable()
            .WithColumn("radix").AsString(32).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("is_hidden").AsBoolean().Nullable()
            .WithColumn("target_name").AsString(64).Nullable()
            .WithColumn("bit_number").AsByte().Nullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        IfDatabase(ProcessorIdConstants.SqlServer)
            .Create.PrimaryKey()
            .OnTable("data_type_member")
            .Column("member_id")
            .NonClustered();

        IfDatabase(ProcessorIdConstants.SqlServer)
            .Create
            .Index().OnTable("data_type_member")
            .OnColumn("type_id").Ascending()
            .OnColumn("member_name").Ascending()
            .WithOptions().Unique()
            .WithOptions().Clustered();

        IfDatabase(ProcessorIdConstants.SQLite)
            .Create
            .Index().OnTable("data_type_member")
            .OnColumn("type_id").Ascending()
            .OnColumn("member_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("data_type_member")
            .OnColumn("type_id").Ascending()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("data_type_member")
            .OnColumn("member_name").Ascending();
    }
}