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
            .WithPrimaryKey<long>("tag_id")
            .WithColumn("program_name").AsString(256).Nullable()
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("data_type").AsString(128).Nullable()
            .WithColumn("dimensions").AsString(32).Nullable()
            .WithColumn("radix").AsString(32).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("opcua_access").AsString(32).Nullable()
            .WithColumn("is_constant").AsBoolean().Nullable()
            .WithColumn("tag_usage").AsString(32).Nullable()
            .WithColumn("tag_type").AsString(32).Nullable()
            .WithColumn("alias_for").AsString(256).NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("tag")
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag")
            .OnColumn("program_name").Ascending()
            .OnColumn("tag_name").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag")
            .OnColumn("tag_name").Ascending();

        Create.Index().OnTable("tag")
            .OnColumn("data_type").Ascending();
    }
}