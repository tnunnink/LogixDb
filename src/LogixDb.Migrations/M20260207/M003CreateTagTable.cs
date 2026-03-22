using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260207;

[UsedImplicitly]
[Migration(202602070830, "Creates tag table with corresponding indexes and keys")]
public class M003CreateTagTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag")
            .WithPrimaryGuid("tag_id")
            .WithNumericCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("program_name").AsString(128).NotNullable()
            .WithColumn("tag_name").AsString(256).NotNullable()
            .WithColumn("base_name").AsString(128).NotNullable()
            .WithColumn("parent_name").AsString(256).Nullable()
            .WithColumn("member_name").AsString(128).NotNullable()
            .WithColumn("data_type").AsString(128).Nullable()
            .WithColumn("tag_value").AsString(256).Nullable()
            .WithColumn("tag_usage").AsString(32).Nullable()
            .WithColumn("external_access").AsString(32).Nullable()
            .WithColumn("is_constant").AsBoolean().Nullable()
            .WithColumn("alias_for").AsString(256).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("tag")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("program_name").Ascending()
            .OnColumn("tag_name").Ascending()
            .WithOptions().Unique();
        
        Create.Index().OnTable("tag")
            .OnColumn("tag_name").Ascending()
            .OnColumn("snapshot_id").Ascending();
        
        Create.Index().OnTable("tag")
            .OnColumn("data_type").Ascending()
            .OnColumn("snapshot_id").Ascending();
        
        Create.Index().OnTable("tag")
            .OnColumn("base_name").Ascending()
            .OnColumn("snapshot_id").Ascending();
        
        Create.Index().OnTable("tag")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("parent_name").Ascending();
        
        Create.Index().OnTable("tag")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("member_name").Ascending();

        Create.Index().OnTable("tag")
            .OnColumn("record_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}