using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Core.Maps;

namespace LogixDb.Migrations.M20260207;

[UsedImplicitly]
[Migration(202602070830, "Creates tag table with corresponding indexes and keys")]
public class M01CreateTagTable : AutoReversingMigration
{
    public override void Up()
    {
        var map = new TagMap();

        Create.Table(map.TableName)
            .WithColumn("tag_id").AsInt32().PrimaryKey().Identity()
            .WithColumn("snapshot_id").AsInt32().NotNullable().ForeignKey("snapshot", "snapshot_id")
            .OnDelete(Rule.Cascade)
            .WithColumn("reference").AsString(256).NotNullable()
            .WithColumn("base_name").AsString(64).NotNullable()
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("scope_level").AsString(64).NotNullable()
            .WithColumn("scope_name").AsString(64).NotNullable()
            .WithColumn("tag_depth").AsInt16().NotNullable()
            .WithColumn("tag_usage").AsString(64).Nullable()
            .WithColumn("data_type").AsString(64).Nullable()
            .WithColumn("value").AsString(1024).Nullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("dimensions").AsString(32).Nullable()
            .WithColumn("radix").AsString(32).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("opcua_access").AsString(32).Nullable()
            .WithColumn("constant").AsBoolean().Nullable()
            .WithColumn("tag_type").AsString(32).Nullable()
            .WithColumn("alias_for").AsString(256).Nullable()
            .WithColumn("component_class").AsString(32).Nullable();

        Create.Index()
            .OnTable("tag")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("reference").Ascending()
            .WithOptions().Unique();

        Create.Index()
            .OnTable("tag")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("tag_name").Ascending();

        Create.Index()
            .OnTable("tag")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("scope_name").Ascending();
    }
}