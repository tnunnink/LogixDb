using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260213;

[UsedImplicitly]
[Migration(202602130830, "Creates tag table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202602130830CreateTagTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag")
            .WithPrimaryKey("tag_id")
            .WithRelation("instance_id", "target_instance").OnDelete(Rule.Cascade).NotNullable()
            .WithRelation("program_id", "program").Nullable()
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("data_type").AsString(128).Nullable()
            .WithColumn("dimensions").AsString(32).Nullable()
            .WithColumn("radix").AsString(32).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("opcua_access").AsString(32).Nullable()
            .WithColumn("is_constant").AsBoolean().Nullable()
            .WithColumn("tag_usage").AsString(32).Nullable()
            .WithColumn("tag_type").AsString(32).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable()
            .WithColumn("source_hash").AsString(32).NotNullable();

        Create.Index().OnTable("tag")
            .OnColumn("instance_id").Ascending()
            .OnColumn("program_id").Ascending()
            .OnColumn("tag_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag")
            .OnColumn("tag_name").Ascending()
            .OnColumn("instance_id").Ascending();

        Create.Index().OnTable("tag")
            .OnColumn("data_type").Ascending()
            .OnColumn("instance_id").Ascending();

        Create.Index().OnTable("tag")
            .OnColumn("record_hash").Ascending()
            .OnColumn("instance_id").Ascending();
    }
}