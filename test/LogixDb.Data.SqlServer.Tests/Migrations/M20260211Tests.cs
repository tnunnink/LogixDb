namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260211Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM202602111430_CreatesControllerTableWithExpectedColumns()
    {
        await Database.Migrate(202602111430);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("controller");

            await AssertColumnDefinition("controller", "controller_id", "uniqueidentifier");
            await AssertColumnDefinition("controller", "snapshot_id", "int");
            await AssertColumnDefinition("controller", "controller_name", "nvarchar");
            await AssertColumnDefinition("controller", "catalog_number", "nvarchar");
            await AssertColumnDefinition("controller", "revision", "nvarchar");
            await AssertColumnDefinition("controller", "controller_description", "nvarchar");
            await AssertColumnDefinition("controller", "project_creation_date", "datetime");
            await AssertColumnDefinition("controller", "last_modified_date", "datetime");
            await AssertColumnDefinition("controller", "communication_path", "nvarchar");
            await AssertColumnDefinition("controller", "sfc_execution_control", "nvarchar");
            await AssertColumnDefinition("controller", "sfc_restart_position", "nvarchar");
            await AssertColumnDefinition("controller", "sfc_last_scan", "nvarchar");
            await AssertColumnDefinition("controller", "project_serial_number", "nvarchar");
            await AssertColumnDefinition("controller", "match_project_to_controller", "bit");
            await AssertColumnDefinition("controller", "inhibit_firmware_updates", "bit");
            await AssertColumnDefinition("controller", "allow_rfi_from_producer", "bit");
            await AssertColumnDefinition("controller", "pass_through_option", "nvarchar");
            await AssertColumnDefinition("controller", "download_documentation", "bit");
            await AssertColumnDefinition("controller", "download_properties", "bit");
            await AssertColumnDefinition("controller", "ethernet_ip_mode", "nvarchar");
            await AssertColumnDefinition("controller", "redundancy_enabled", "bit");
            await AssertColumnDefinition("controller", "keep_test_edits_on_switch", "bit");
            await AssertColumnDefinition("controller", "io_memory_pad_percent", "real");
            await AssertColumnDefinition("controller", "data_table_pad_percent", "real");
            await AssertColumnDefinition("controller", "safety_signature", "nvarchar");
            await AssertColumnDefinition("controller", "safety_lock_password", "nvarchar");
            await AssertColumnDefinition("controller", "safety_unlock_password", "nvarchar");
            await AssertColumnDefinition("controller", "configure_safety_io_always", "bit");
            await AssertColumnDefinition("controller", "signature_run_mode_protect", "bit");
            await AssertColumnDefinition("controller", "security_authority_id", "nvarchar");
            await AssertColumnDefinition("controller", "security_authority_uri", "nvarchar");
            await AssertColumnDefinition("controller", "permission_set", "nvarchar");
            await AssertColumnDefinition("controller", "changed_to_detect", "nvarchar");
            await AssertColumnDefinition("controller", "trusted_slots", "nvarchar");
            await AssertColumnDefinition("controller", "record_hash", "nvarchar");

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
            await AssertColumnDefinition("data_type", "snapshot_id", "int");
            await AssertColumnDefinition("data_type", "type_name", "nvarchar");
            await AssertColumnDefinition("data_type", "type_class", "nvarchar");
            await AssertColumnDefinition("data_type", "type_family", "nvarchar");
            await AssertColumnDefinition("data_type", "type_description", "nvarchar");
            await AssertColumnDefinition("data_type", "record_hash", "nvarchar");
            await AssertColumnDefinition("data_type", "source_hash", "nvarchar");

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
            await AssertColumnDefinition("data_type_member", "member_name", "nvarchar");
            await AssertColumnDefinition("data_type_member", "data_type", "nvarchar");
            await AssertColumnDefinition("data_type_member", "dimensions", "nvarchar");
            await AssertColumnDefinition("data_type_member", "radix", "nvarchar");
            await AssertColumnDefinition("data_type_member", "external_access", "nvarchar");
            await AssertColumnDefinition("data_type_member", "member_description", "nvarchar");
            await AssertColumnDefinition("data_type_member", "is_hidden", "bit");
            await AssertColumnDefinition("data_type_member", "target_name", "nvarchar");
            await AssertColumnDefinition("data_type_member", "bit_number", "tinyint");
            await AssertColumnDefinition("data_type_member", "record_hash", "nvarchar");

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
            await AssertColumnDefinition("task", "snapshot_id", "int");
            await AssertColumnDefinition("task", "task_name", "nvarchar");
            await AssertColumnDefinition("task", "task_type", "nvarchar");
            await AssertColumnDefinition("task", "task_description", "nvarchar");
            await AssertColumnDefinition("task", "priority", "tinyint");
            await AssertColumnDefinition("task", "scan_rate", "real");
            await AssertColumnDefinition("task", "watchdog", "real");
            await AssertColumnDefinition("task", "is_inhibited", "bit");
            await AssertColumnDefinition("task", "disable_outputs", "bit");
            await AssertColumnDefinition("task", "event_trigger", "nvarchar");
            await AssertColumnDefinition("task", "event_tag", "nvarchar");
            await AssertColumnDefinition("task", "enable_timeout", "bit");
            await AssertColumnDefinition("task", "record_hash", "nvarchar");
            await AssertColumnDefinition("task", "source_hash", "nvarchar");

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
            await AssertColumnDefinition("program", "snapshot_id", "int");
            await AssertColumnDefinition("program", "task_id", "uniqueidentifier");
            await AssertColumnDefinition("program", "folder_id", "uniqueidentifier");
            await AssertColumnDefinition("program", "program_name", "nvarchar");
            await AssertColumnDefinition("program", "program_type", "nvarchar");
            await AssertColumnDefinition("program", "program_description", "nvarchar");
            await AssertColumnDefinition("program", "main_routine", "nvarchar");
            await AssertColumnDefinition("program", "fault_routine", "nvarchar");
            await AssertColumnDefinition("program", "is_disabled", "bit");
            await AssertColumnDefinition("program", "is_folder", "bit");
            await AssertColumnDefinition("program", "has_test_edits", "bit");
            await AssertColumnDefinition("program", "record_hash", "nvarchar");
            await AssertColumnDefinition("program", "source_hash", "nvarchar");

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
            await AssertColumnDefinition("routine", "snapshot_id", "int");
            await AssertColumnDefinition("routine", "program_id", "uniqueidentifier");
            await AssertColumnDefinition("routine", "routine_name", "nvarchar");
            await AssertColumnDefinition("routine", "routine_type", "nvarchar");
            await AssertColumnDefinition("routine", "routine_description", "nvarchar");
            await AssertColumnDefinition("routine", "record_hash", "nvarchar");
            await AssertColumnDefinition("routine", "source_hash", "nvarchar");

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
            await AssertColumnDefinition("rung", "snapshot_id", "int");
            await AssertColumnDefinition("rung", "routine_id", "uniqueidentifier");
            await AssertColumnDefinition("rung", "rung_number", "int");
            await AssertColumnDefinition("rung", "rung_text", "nvarchar");
            await AssertColumnDefinition("rung", "rung_comment", "nvarchar");
            await AssertColumnDefinition("rung", "record_hash", "nvarchar");

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
            await AssertColumnDefinition("module", "snapshot_id", "int");
            await AssertColumnDefinition("module", "parent_id", "uniqueidentifier");
            await AssertColumnDefinition("module", "module_name", "nvarchar");
            await AssertColumnDefinition("module", "catalog_number", "nvarchar");
            await AssertColumnDefinition("module", "revision", "nvarchar");
            await AssertColumnDefinition("module", "module_description", "nvarchar");
            await AssertColumnDefinition("module", "vendor_id", "int");
            await AssertColumnDefinition("module", "product_id", "int");
            await AssertColumnDefinition("module", "product_code", "smallint");
            await AssertColumnDefinition("module", "parent_name", "nvarchar");
            await AssertColumnDefinition("module", "parent_port", "tinyint");
            await AssertColumnDefinition("module", "electronic_keying", "nvarchar");
            await AssertColumnDefinition("module", "is_inhibited", "bit");
            await AssertColumnDefinition("module", "is_major_fault_enabled", "bit");
            await AssertColumnDefinition("module", "is_safety_enabled", "bit");
            await AssertColumnDefinition("module", "ip_address", "nvarchar");
            await AssertColumnDefinition("module", "slot_number", "tinyint");
            await AssertColumnDefinition("module", "record_hash", "nvarchar");
            await AssertColumnDefinition("module", "source_hash", "nvarchar");

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