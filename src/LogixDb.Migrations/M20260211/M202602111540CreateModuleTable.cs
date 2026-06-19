using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111540, "Creates module table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Module)]
public class M202602111540CreateModuleTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("module")
            .WithPrimaryKey<long>("module_id")
            .WithColumn("module_name").AsString(256).NotNullable()
            .WithColumn("module_description").AsString(512).Nullable()
            .WithColumn("catalog_number").AsString(64).Nullable()
            .WithColumn("revision").AsString(16).Nullable()
            .WithColumn("vendor_id").AsInt32().Nullable()
            .WithColumn("product_id").AsInt32().Nullable()
            .WithColumn("product_code").AsInt16().Nullable()
            .WithColumn("parent_name").AsString(256).NotNullable()
            .WithColumn("parent_port").AsByte().NotNullable()
            .WithColumn("electronic_keying").AsString(32).Nullable()
            .WithColumn("is_inhibited").AsBoolean().Nullable()
            .WithColumn("is_major_fault_enabled").AsBoolean().Nullable()
            .WithColumn("is_safety_enabled").AsBoolean().Nullable()
            .WithColumn("config_tag").AsString(256).Nullable()
            .WithColumn("ip_address").AsString(32).Nullable()
            .WithColumn("slot_number").AsByte().Nullable()
            .WithColumn("content_hash").AsString(64).NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("module")
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("module")
            .OnColumn("module_name").Ascending();

        Create.Index().OnTable("module")
            .OnColumn("parent_name").Ascending();
    }
}