namespace LogixDb.Data.SqlServer.Tests.Migrations;

[TestFixture]
public class M20260211Tests : SqlServerTestFixture
{
    [Test]
    public async Task MigrateUp_ToM004_CreatesControllerTableWithExpectedColumns()
    {
        await Database.Migrate(202602111430);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("controller");

            await AssertColumnDefinition("controller", "controller_id", "int");
            await AssertColumnDefinition("controller", "snapshot_id", "int");
            await AssertColumnDefinition("controller", "controller_name", "nvarchar");
            await AssertColumnDefinition("controller", "catalog_number", "nvarchar");
            await AssertColumnDefinition("controller", "controller_revision", "nvarchar");
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
            await AssertColumnDefinition("controller", "record_hash", "nvarchar");

            await AssertPrimaryKey("controller", "controller_id");
            await AssertForeignKey("controller", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("controller", "controller_name", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM005_CreatesDataTypeTableWithExpectedColumns()
    {
        await Database.Migrate(202602111500);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("data_type");

            await AssertColumnDefinition("data_type", "type_id", "int");
            await AssertColumnDefinition("data_type", "snapshot_id", "int");
            await AssertColumnDefinition("data_type", "type_name", "nvarchar");
            await AssertColumnDefinition("data_type", "type_class", "nvarchar");
            await AssertColumnDefinition("data_type", "type_family", "nvarchar");
            await AssertColumnDefinition("data_type", "type_description", "nvarchar");
            await AssertColumnDefinition("data_type", "record_hash", "nvarchar");

            await AssertPrimaryKey("data_type", "type_id");
            await AssertForeignKey("data_type", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("data_type", "snapshot_id", "type_name");
            await AssertIndex("data_type", "record_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM006_CreatesDataTypeMemberTableWithExpectedColumns()
    {
        await Database.Migrate(202602111530);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("data_type_member");

            await AssertColumnDefinition("data_type_member", "member_id", "int");
            await AssertColumnDefinition("data_type_member", "snapshot_id", "int");
            await AssertColumnDefinition("data_type_member", "type_name", "nvarchar");
            await AssertColumnDefinition("data_type_member", "member_name", "nvarchar");
            await AssertColumnDefinition("data_type_member", "data_type", "nvarchar");
            await AssertColumnDefinition("data_type_member", "dimension", "smallint");
            await AssertColumnDefinition("data_type_member", "radix", "nvarchar");
            await AssertColumnDefinition("data_type_member", "external_access", "nvarchar");
            await AssertColumnDefinition("data_type_member", "member_description", "nvarchar");
            await AssertColumnDefinition("data_type_member", "is_hidden", "bit");
            await AssertColumnDefinition("data_type_member", "target_name", "nvarchar");
            await AssertColumnDefinition("data_type_member", "bit_number", "tinyint");
            await AssertColumnDefinition("data_type_member", "record_hash", "nvarchar");

            await AssertPrimaryKey("data_type_member", "member_id");
            await AssertForeignKey("data_type_member", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("data_type_member", "snapshot_id", "type_name", "member_name");
        }
    }

    [Test]
    public async Task MigrateUp_ToM007_CreatesTaskTableWithExpectedColumns()
    {
        await Database.Migrate(202602111600);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("task");

            await AssertColumnDefinition("task", "task_id", "int");
            await AssertColumnDefinition("task", "snapshot_id", "int");
            await AssertColumnDefinition("task", "task_name", "nvarchar");
            await AssertColumnDefinition("task", "task_type", "nvarchar");
            await AssertColumnDefinition("task", "task_description", "nvarchar");
            await AssertColumnDefinition("task", "task_priority", "tinyint");
            await AssertColumnDefinition("task", "task_rate", "real");
            await AssertColumnDefinition("task", "task_watchdog", "real");
            await AssertColumnDefinition("task", "is_inhibited", "bit");
            await AssertColumnDefinition("task", "disable_outputs", "bit");
            await AssertColumnDefinition("task", "event_trigger", "nvarchar");
            await AssertColumnDefinition("task", "event_tag", "nvarchar");
            await AssertColumnDefinition("task", "enable_timeout", "bit");
            await AssertColumnDefinition("task", "record_hash", "nvarchar");

            await AssertPrimaryKey("task", "task_id");
            await AssertForeignKey("task", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("task", "snapshot_id", "task_name");
            await AssertIndex("task", "record_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM008_CreatesProgramTableWithExpectedColumns()
    {
        await Database.Migrate(202602111630);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("program");

            await AssertColumnDefinition("program", "program_id", "int");
            await AssertColumnDefinition("program", "snapshot_id", "int");
            await AssertColumnDefinition("program", "program_name", "nvarchar");
            await AssertColumnDefinition("program", "program_type", "nvarchar");
            await AssertColumnDefinition("program", "program_description", "nvarchar");
            await AssertColumnDefinition("program", "main_routine", "nvarchar");
            await AssertColumnDefinition("program", "fault_routine", "nvarchar");
            await AssertColumnDefinition("program", "is_disabled", "bit");
            await AssertColumnDefinition("program", "is_folder", "bit");
            await AssertColumnDefinition("program", "has_test_edits", "bit");
            await AssertColumnDefinition("program", "parent_name", "nvarchar");
            await AssertColumnDefinition("program", "task_name", "nvarchar");
            await AssertColumnDefinition("program", "record_hash", "nvarchar");

            await AssertPrimaryKey("program", "program_id");
            await AssertForeignKey("program", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("program", "snapshot_id", "program_name");
            await AssertIndex("program", "record_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM009_CreatesRoutineTableWithExpectedColumns()
    {
        await Database.Migrate(202602111930);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("routine");

            await AssertColumnDefinition("routine", "routine_id", "int");
            await AssertColumnDefinition("routine", "snapshot_id", "int");
            await AssertColumnDefinition("routine", "program_name", "nvarchar");
            await AssertColumnDefinition("routine", "routine_name", "nvarchar");
            await AssertColumnDefinition("routine", "routine_type", "nvarchar");
            await AssertColumnDefinition("routine", "routine_description", "nvarchar");
            await AssertColumnDefinition("routine", "record_hash", "nvarchar");

            await AssertPrimaryKey("routine", "routine_id");
            await AssertForeignKey("routine", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("routine", "snapshot_id", "program_name", "routine_name");
            await AssertIndex("routine", "record_hash", "snapshot_id");
        }
    }

    [Test]
    public async Task MigrateUp_ToM010_CreatesRungTableWithExpectedColumns()
    {
        await Database.Migrate(202602111945);

        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("rung");

            await AssertColumnDefinition("rung", "rung_id", "int");
            await AssertColumnDefinition("rung", "snapshot_id", "int");
            await AssertColumnDefinition("rung", "program_name", "nvarchar");
            await AssertColumnDefinition("rung", "routine_name", "nvarchar");
            await AssertColumnDefinition("rung", "rung_number", "int");
            await AssertColumnDefinition("rung", "rung_comment", "nvarchar");
            await AssertColumnDefinition("rung", "rung_text", "nvarchar");
            await AssertColumnDefinition("rung", "record_hash", "nvarchar");

            await AssertPrimaryKey("rung", "rung_id");
            await AssertForeignKey("rung", "snapshot_id", "snapshot", "snapshot_id");
            await AssertUniqueIndex("rung", "snapshot_id", "program_name", "routine_name", "rung_number");
            await AssertIndex("rung", "record_hash", "snapshot_id");
        }
    }
}