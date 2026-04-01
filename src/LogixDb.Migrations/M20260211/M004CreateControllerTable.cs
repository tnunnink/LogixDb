using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260211;

[UsedImplicitly]
[Migration(202602111430, "Creates controller table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Controller)]
public class M004CreateControllerTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("controller")
            .WithPrimaryGuid("controller_id")
            .WithNumericCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("controller_name").AsString(128).NotNullable()
            .WithColumn("catalog_number").AsString(128).Nullable()
            .WithColumn("controller_revision").AsString(32).Nullable()
            .WithColumn("controller_description").AsString(512).Nullable()
            .WithColumn("project_creation_date").AsDateTime().Nullable()
            .WithColumn("last_modified_date").AsDateTime().Nullable()
            .WithColumn("communication_path").AsString(128).Nullable()
            .WithColumn("sfc_execution_control").AsString(32).Nullable()
            .WithColumn("sfc_restart_position").AsString(32).Nullable()
            .WithColumn("sfc_last_scan").AsString(32).Nullable()
            .WithColumn("project_serial_number").AsString(32).Nullable()
            .WithColumn("match_project_to_controller").AsBoolean().Nullable()
            .WithColumn("inhibit_firmware_updates").AsBoolean().Nullable()
            .WithColumn("allow_rfi_from_producer").AsBoolean().Nullable()
            .WithColumn("pass_through_option").AsString(32).Nullable()
            .WithColumn("download_documentation").AsBoolean().Nullable()
            .WithColumn("download_properties").AsBoolean().Nullable()
            .WithColumn("ethernet_ip_mode").AsString(32).Nullable()
            .WithColumn("redundancy_enabled").AsBoolean().Nullable()
            .WithColumn("keep_test_edits_on_switch").AsBoolean().Nullable()
            .WithColumn("io_memory_pad_percent").AsFloat().Nullable()
            .WithColumn("data_table_pad_percent").AsFloat().Nullable()
            .WithColumn("safety_signature").AsString(32).Nullable()
            .WithColumn("safety_lock_password").AsString(32).Nullable()
            .WithColumn("safety_unlock_password").AsString(32).Nullable()
            .WithColumn("configure_safety_io_always").AsBoolean().Nullable()
            .WithColumn("signature_run_mode_protect").AsBoolean().Nullable()
            .WithColumn("security_authority_id").AsString().Nullable()
            .WithColumn("security_authority_uri").AsString().Nullable()
            .WithColumn("permission_set").AsString().Nullable()
            .WithColumn("changed_to_detect").AsString().Nullable()
            .WithColumn("trusted_slots").AsString(64).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index()
            .OnTable("controller")
            .OnColumn("controller_name").Ascending()
            .OnColumn("snapshot_id").Ascending()
            .WithOptions().Unique();
    }
}