namespace LogixDb.Data.Sqlite.Tests.Migrations;

[TestFixture]
public class M20260211Tests : SqliteTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602111430_CreatesControllerTableWithExpectedColumns()
    {
        await Database.Migrate(202602111430);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("controller");

            await AssertColumnDefinition("controller", "controller_id", "uniqueidentifier");
            await AssertColumnDefinition("controller", "snapshot_id", "integer");
            await AssertColumnDefinition("controller", "controller_name", "text");
            await AssertColumnDefinition("controller", "catalog_number", "text");
            await AssertColumnDefinition("controller", "revision", "text");
            await AssertColumnDefinition("controller", "controller_description", "text");
            await AssertColumnDefinition("controller", "project_creation_date", "datetime");
            await AssertColumnDefinition("controller", "last_modified_date", "datetime");
            await AssertColumnDefinition("controller", "communication_path", "text");
            await AssertColumnDefinition("controller", "sfc_execution_control", "text");
            await AssertColumnDefinition("controller", "sfc_restart_position", "text");
            await AssertColumnDefinition("controller", "sfc_last_scan", "text");
            await AssertColumnDefinition("controller", "project_serial_number", "text");
            await AssertColumnDefinition("controller", "match_project_to_controller", "integer");
            await AssertColumnDefinition("controller", "inhibit_firmware_updates", "integer");
            await AssertColumnDefinition("controller", "allow_rfi_from_producer", "integer");
            await AssertColumnDefinition("controller", "pass_through_option", "text");
            await AssertColumnDefinition("controller", "download_documentation", "integer");
            await AssertColumnDefinition("controller", "download_properties", "integer");
            await AssertColumnDefinition("controller", "ethernet_ip_mode", "text");
            await AssertColumnDefinition("controller", "redundancy_enabled", "integer");
            await AssertColumnDefinition("controller", "keep_test_edits_on_switch", "integer");
            await AssertColumnDefinition("controller", "io_memory_pad_percent", "numeric");
            await AssertColumnDefinition("controller", "data_table_pad_percent", "numeric");
            await AssertColumnDefinition("controller", "safety_signature", "text");
            await AssertColumnDefinition("controller", "safety_lock_password", "text");
            await AssertColumnDefinition("controller", "safety_unlock_password", "text");
            await AssertColumnDefinition("controller", "configure_safety_io_always", "integer");
            await AssertColumnDefinition("controller", "signature_run_mode_protect", "integer");
            await AssertColumnDefinition("controller", "security_authority_id", "text");
            await AssertColumnDefinition("controller", "security_authority_uri", "text");
            await AssertColumnDefinition("controller", "permission_set", "text");
            await AssertColumnDefinition("controller", "changed_to_detect", "text");
            await AssertColumnDefinition("controller", "trusted_slots", "text");
            await AssertColumnDefinition("controller", "record_hash", "text");

            await AssertPrimaryKey("controller", "controller_id");
            await AssertForeignKey("controller", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("controller", "controller_name", "snapshot_id");
            await AssertIndex("controller", "controller_name", "record_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602111500_CreatesDataTypeTableWithExpectedColumns()
    {
        await Database.Migrate(202602111500);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("data_type");

            await AssertColumnDefinition("data_type", "type_id", "uniqueidentifier");
            await AssertColumnDefinition("data_type", "snapshot_id", "integer");
            await AssertColumnDefinition("data_type", "type_name", "text");
            await AssertColumnDefinition("data_type", "type_class", "text");
            await AssertColumnDefinition("data_type", "type_family", "text");
            await AssertColumnDefinition("data_type", "type_description", "text");
            await AssertColumnDefinition("data_type", "record_hash", "text");
            await AssertColumnDefinition("data_type", "source_hash", "text");

            await AssertPrimaryKey("data_type", "type_id");
            await AssertForeignKey("data_type", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("data_type", "snapshot_id", "type_name");
            await AssertIndex("data_type", "type_name", "record_hash");
            await AssertIndex("data_type", "source_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602111530_CreatesDataTypeMemberTableWithExpectedColumns()
    {
        await Database.Migrate(202602111530);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("data_type_member");

            await AssertColumnDefinition("data_type_member", "member_id", "uniqueidentifier");
            await AssertColumnDefinition("data_type_member", "type_id", "uniqueidentifier");
            await AssertColumnDefinition("data_type_member", "member_name", "text");
            await AssertColumnDefinition("data_type_member", "data_type", "text");
            await AssertColumnDefinition("data_type_member", "dimensions", "text");
            await AssertColumnDefinition("data_type_member", "radix", "text");
            await AssertColumnDefinition("data_type_member", "external_access", "text");
            await AssertColumnDefinition("data_type_member", "member_description", "text");
            await AssertColumnDefinition("data_type_member", "is_hidden", "integer");
            await AssertColumnDefinition("data_type_member", "target_name", "text");
            await AssertColumnDefinition("data_type_member", "bit_number", "integer");
            await AssertColumnDefinition("data_type_member", "record_hash", "text");

            await AssertPrimaryKey("data_type_member", "member_id");
            await AssertForeignKey("data_type_member", "type_id", "data_type", "type_id");
            await AssertUniqueIndex("data_type_member", "type_id", "member_name");
            await AssertIndex("data_type_member", "member_name", "record_hash");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602111600_CreatesTaskTableWithExpectedColumns()
    {
        await Database.Migrate(202602111600);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("task");

            await AssertColumnDefinition("task", "task_id", "uniqueidentifier");
            await AssertColumnDefinition("task", "snapshot_id", "integer");
            await AssertColumnDefinition("task", "task_name", "text");
            await AssertColumnDefinition("task", "task_type", "text");
            await AssertColumnDefinition("task", "task_description", "text");
            await AssertColumnDefinition("task", "priority", "integer");
            await AssertColumnDefinition("task", "scan_rate", "numeric");
            await AssertColumnDefinition("task", "watchdog", "numeric");
            await AssertColumnDefinition("task", "is_inhibited", "integer");
            await AssertColumnDefinition("task", "disable_outputs", "integer");
            await AssertColumnDefinition("task", "event_trigger", "text");
            await AssertColumnDefinition("task", "event_tag", "text");
            await AssertColumnDefinition("task", "enable_timeout", "integer");
            await AssertColumnDefinition("task", "record_hash", "text");
            await AssertColumnDefinition("task", "source_hash", "text");

            await AssertPrimaryKey("task", "task_id");
            await AssertForeignKey("task", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("task", "snapshot_id", "task_name");
            await AssertIndex("task", "task_name", "record_hash");
            await AssertIndex("task", "source_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602111630_CreatesProgramTableWithExpectedColumns()
    {
        await Database.Migrate(202602111630);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("program");

            await AssertColumnDefinition("program", "program_id", "uniqueidentifier");
            await AssertColumnDefinition("program", "snapshot_id", "integer");
            await AssertColumnDefinition("program", "task_id", "uniqueidentifier");
            await AssertColumnDefinition("program", "folder_id", "uniqueidentifier");
            await AssertColumnDefinition("program", "program_name", "text");
            await AssertColumnDefinition("program", "program_type", "text");
            await AssertColumnDefinition("program", "program_description", "text");
            await AssertColumnDefinition("program", "main_routine", "text");
            await AssertColumnDefinition("program", "fault_routine", "text");
            await AssertColumnDefinition("program", "is_disabled", "integer");
            await AssertColumnDefinition("program", "is_folder", "integer");
            await AssertColumnDefinition("program", "has_test_edits", "integer");
            await AssertColumnDefinition("program", "record_hash", "text");
            await AssertColumnDefinition("program", "source_hash", "text");

            await AssertPrimaryKey("program", "program_id");
            await AssertForeignKey("program", "snapshot_id", "snapshot", "snapshot_id");
            await AssertForeignKey("program", "task_id", "task", "task_id");
            await AssertForeignKey("program", "folder_id", "program", "program_id");
            await AssertUniqueIndex("program", "snapshot_id", "program_name");
            await AssertIndex("program", "folder_id", "snapshot_id");
            await AssertIndex("program", "program_name", "record_hash");
            await AssertIndex("program", "source_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602111930_CreatesRoutineTableWithExpectedColumns()
    {
        await Database.Migrate(202602111930);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("routine");

            await AssertColumnDefinition("routine", "routine_id", "uniqueidentifier");
            await AssertColumnDefinition("routine", "snapshot_id", "integer");
            await AssertColumnDefinition("routine", "program_id", "uniqueidentifier");
            await AssertColumnDefinition("routine", "routine_name", "text");
            await AssertColumnDefinition("routine", "routine_type", "text");
            await AssertColumnDefinition("routine", "routine_description", "text");
            await AssertColumnDefinition("routine", "record_hash", "text");
            await AssertColumnDefinition("routine", "source_hash", "text");

            await AssertPrimaryKey("routine", "routine_id");
            await AssertForeignKey("routine", "snapshot_id", "snapshot", "snapshot_id");
            await AssertForeignKey("routine", "program_id", "program", "program_id");
            await AssertUniqueIndex("routine", "program_id", "routine_name");
            await AssertIndex("routine", "routine_name", "record_hash");
            await AssertIndex("routine", "source_hash", "program_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602111945_CreatesRungTableWithExpectedColumns()
    {
        await Database.Migrate(202602111945);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung");

            await AssertColumnDefinition("rung", "rung_id", "uniqueidentifier");
            await AssertColumnDefinition("rung", "snapshot_id", "integer");
            await AssertColumnDefinition("rung", "routine_id", "uniqueidentifier");
            await AssertColumnDefinition("rung", "rung_number", "integer");
            await AssertColumnDefinition("rung", "rung_text", "text");
            await AssertColumnDefinition("rung", "rung_comment", "text");
            await AssertColumnDefinition("rung", "record_hash", "text");

            await AssertPrimaryKey("rung", "rung_id");
            await AssertForeignKey("rung", "snapshot_id", "snapshot", "snapshot_id");
            await AssertForeignKey("rung", "routine_id", "routine", "routine_id");
            await AssertIndex("rung", "record_hash", "routine_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM202602111540_CreatesModuleTableWithExpectedColumns()
    {
        await Database.Migrate(202602111540);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("module");

            await AssertColumnDefinition("module", "module_id", "uniqueidentifier");
            await AssertColumnDefinition("module", "snapshot_id", "integer");
            await AssertColumnDefinition("module", "parent_id", "uniqueidentifier");
            await AssertColumnDefinition("module", "module_name", "text");
            await AssertColumnDefinition("module", "catalog_number", "text");
            await AssertColumnDefinition("module", "revision", "text");
            await AssertColumnDefinition("module", "module_description", "text");
            await AssertColumnDefinition("module", "vendor_id", "integer");
            await AssertColumnDefinition("module", "product_id", "integer");
            await AssertColumnDefinition("module", "product_code", "integer");
            await AssertColumnDefinition("module", "parent_name", "text");
            await AssertColumnDefinition("module", "parent_port", "integer");
            await AssertColumnDefinition("module", "electronic_keying", "text");
            await AssertColumnDefinition("module", "is_inhibited", "integer");
            await AssertColumnDefinition("module", "is_major_fault_enabled", "integer");
            await AssertColumnDefinition("module", "is_safety_enabled", "integer");
            await AssertColumnDefinition("module", "ip_address", "text");
            await AssertColumnDefinition("module", "slot_number", "integer");
            await AssertColumnDefinition("module", "record_hash", "text");
            await AssertColumnDefinition("module", "source_hash", "text");

            await AssertPrimaryKey("module", "module_id");
            await AssertForeignKey("module", "snapshot_id", "snapshot", "snapshot_id");
            await AssertForeignKey("module", "parent_id", "module", "module_id");
            await AssertUniqueIndex("module", "snapshot_id", "module_name");
            await AssertIndex("module", "parent_id", "snapshot_id");
            await AssertIndex("module", "module_name", "record_hash");
            await AssertIndex("module", "source_hash", "snapshot_id");
        }
    }
}