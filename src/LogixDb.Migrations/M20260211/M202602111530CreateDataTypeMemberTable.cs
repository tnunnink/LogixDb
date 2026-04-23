using System.Data;
using FluentMigrator;
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
        Create.Table("data_type_member")
            .WithPrimaryKey("member_id")
            .WithRelation<int>("instance_id", "target_instance").OnDelete(Rule.Cascade).Nullable()
            .WithRelation<Guid>("type_id", "data_type").NotNullable()
            .WithColumn("member_name").AsString(256).NotNullable()
            .WithColumn("member_description").AsString(512).Nullable()
            .WithColumn("data_type").AsString(256).Nullable()
            .WithColumn("dimensions").AsString(32).Nullable()
            .WithColumn("radix").AsString(32).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("is_hidden").AsBoolean().Nullable()
            .WithColumn("target_name").AsString(64).Nullable()
            .WithColumn("bit_number").AsByte().Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("data_type_member")
            .OnColumn("type_id").Ascending()
            .OnColumn("member_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("data_type_member")
            .OnColumn("member_name").Ascending()
            .OnColumn("record_hash").Ascending();
    }
}