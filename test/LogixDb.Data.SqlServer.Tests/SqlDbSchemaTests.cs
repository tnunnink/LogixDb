namespace LogixDb.Data.SqlServer.Tests;

[TestFixture]
public class SqlDbSchemaTests : SqlServerTestFixture
{
    [Test]
    public async Task TargetTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target");

            await AssertColumnDefinition("target", "target_id", "int");
            await AssertColumnDefinition("target", "target_key", "nvarchar");
            await AssertColumnDefinition("target", "created_on", "datetime");

            await AssertPrimaryKey("target", "target_id");
            await AssertUniqueIndex("target", "target_key");
        }
    }

    [Test]
    public async Task TargetVersionTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target_version");

            await AssertColumnDefinition("target_version", "version_id", "int");
            await AssertColumnDefinition("target_version", "target_id", "int");
            await AssertColumnDefinition("target_version", "version_number", "int");
            await AssertColumnDefinition("target_version", "target_type", "nvarchar");
            await AssertColumnDefinition("target_version", "target_name", "nvarchar");
            await AssertColumnDefinition("target_version", "is_partial", "bit");
            await AssertColumnDefinition("target_version", "schema_revision", "nvarchar");
            await AssertColumnDefinition("target_version", "software_revision", "nvarchar");
            await AssertColumnDefinition("target_version", "export_date", "datetime");
            await AssertColumnDefinition("target_version", "export_options", "nvarchar");
            await AssertColumnDefinition("target_version", "import_date", "datetime");
            await AssertColumnDefinition("target_version", "import_user", "nvarchar");
            await AssertColumnDefinition("target_version", "import_machine", "nvarchar");
            await AssertColumnDefinition("target_version", "source_hash", "nvarchar");
            await AssertColumnDefinition("target_version", "source_data", "varbinary");

            await AssertPrimaryKey("target_version", "version_id");
            await AssertForeignKey("target_version", "target_id", "target", "target_id");
        }
    }

    [Test]
    public async Task TargetVersionMapTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target_version_map");

            await AssertColumnDefinition("target_version_map", "version_id", "int");
            await AssertColumnDefinition("target_version_map", "record_id", "bigint");
            await AssertColumnDefinition("target_version_map", "component_id", "tinyint");

            await AssertForeignKey("target_version_map", "version_id", "target_version", "version_id");
            await AssertUniqueIndex("target_version_map", "version_id", "component_id", "record_id");
            await AssertIndex("target_version_map", "record_id", "component_id");
        }
    }

    [Test]
    public async Task TargetInfoTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target_info");

            await AssertColumnDefinition("target_info", "property_id", "uniqueidentifier");
            await AssertColumnDefinition("target_info", "version_id", "int");
            await AssertColumnDefinition("target_info", "property_name", "nvarchar");
            await AssertColumnDefinition("target_info", "property_value", "nvarchar");

            await AssertPrimaryKey("target_info", "property_id");
            await AssertForeignKey("target_info", "version_id", "target_version", "version_id");
            await AssertUniqueIndex("target_info", "version_id", "property_name");
        }
    }

    [Test]
    public async Task TargetComponentTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "target_component");

            await AssertColumnDefinition("target_component", "component_id", "tinyint");
            await AssertColumnDefinition("target_component", "component_name", "nvarchar");

            await AssertPrimaryKey("target_component", "component_id");
            await AssertUniqueIndex("target_component", "component_name");
        }
    }

    [Test]
    public async Task ControllerTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "controller");

            await AssertColumnDefinition("controller", "controller_id", "bigint");
            await AssertColumnDefinition("controller", "controller_name", "nvarchar");
            await AssertColumnDefinition("controller", "catalog_number", "nvarchar");
            await AssertColumnDefinition("controller", "revision", "nvarchar");
            await AssertColumnDefinition("controller", "controller_description", "nvarchar");
            await AssertColumnDefinition("controller", "project_creation_date", "datetime");
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
            await AssertUniqueIndex("controller", "record_hash");
            await AssertIndex("controller", "controller_name");
        }
    }

    [Test]
    public async Task DataTypeTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "data_type");

            await AssertColumnDefinition("data_type", "type_id", "bigint");
            await AssertColumnDefinition("data_type", "type_name", "nvarchar");
            await AssertColumnDefinition("data_type", "type_class", "nvarchar");
            await AssertColumnDefinition("data_type", "type_family", "nvarchar");
            await AssertColumnDefinition("data_type", "type_description", "nvarchar");
            await AssertColumnDefinition("data_type", "record_hash", "nvarchar");

            await AssertPrimaryKey("data_type", "type_id");
            await AssertUniqueIndex("data_type", "record_hash");
            await AssertIndex("data_type", "type_name");
        }
    }

    [Test]
    public async Task DataTypeMemberTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "data_type_member");

            await AssertColumnDefinition("data_type_member", "member_id", "bigint");
            await AssertColumnDefinition("data_type_member", "type_id", "bigint");
            await AssertColumnDefinition("data_type_member", "member_name", "nvarchar");
            await AssertColumnDefinition("data_type_member", "member_description", "nvarchar");
            await AssertColumnDefinition("data_type_member", "member_index", "int");
            await AssertColumnDefinition("data_type_member", "data_type", "nvarchar");
            await AssertColumnDefinition("data_type_member", "dimensions", "nvarchar");
            await AssertColumnDefinition("data_type_member", "radix", "nvarchar");
            await AssertColumnDefinition("data_type_member", "external_access", "nvarchar");
            await AssertColumnDefinition("data_type_member", "is_hidden", "bit");
            await AssertColumnDefinition("data_type_member", "target_name", "nvarchar");
            await AssertColumnDefinition("data_type_member", "bit_number", "tinyint");
            await AssertColumnDefinition("data_type_member", "record_hash", "nvarchar");

            await AssertPrimaryKey("data_type_member", "member_id");
            await AssertUniqueIndex("data_type_member", "type_id", "record_hash");
            await AssertUniqueIndex("data_type_member", "type_id", "member_name");
            await AssertIndex("data_type_member", "member_name");
        }
    }

    [Test]
    public async Task ModuleTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "module");

            await AssertColumnDefinition("module", "module_id", "bigint");
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
            await AssertColumnDefinition("module", "config_tag", "nvarchar");
            await AssertColumnDefinition("module", "ip_address", "nvarchar");
            await AssertColumnDefinition("module", "slot_number", "tinyint");
            await AssertColumnDefinition("module", "record_hash", "nvarchar");

            await AssertPrimaryKey("module", "module_id");
            await AssertUniqueIndex("module", "record_hash");
            await AssertIndex("module", "module_name");
            await AssertIndex("module", "parent_name");
        }
    }

    [Test]
    public async Task TaskTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "task");

            await AssertColumnDefinition("task", "task_id", "bigint");
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

            await AssertPrimaryKey("task", "task_id");
            await AssertUniqueIndex("task", "record_hash");
            await AssertIndex("task", "task_name");
        }
    }

    [Test]
    public async Task ProgramTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "program");

            await AssertColumnDefinition("program", "program_id", "bigint");
            await AssertColumnDefinition("program", "program_name", "nvarchar");
            await AssertColumnDefinition("program", "task_name", "nvarchar");
            await AssertColumnDefinition("program", "folder_name", "nvarchar");
            await AssertColumnDefinition("program", "program_description", "nvarchar");
            await AssertColumnDefinition("program", "program_type", "nvarchar");
            await AssertColumnDefinition("program", "main_routine", "nvarchar");
            await AssertColumnDefinition("program", "fault_routine", "nvarchar");
            await AssertColumnDefinition("program", "is_disabled", "bit");
            await AssertColumnDefinition("program", "is_folder", "bit");
            await AssertColumnDefinition("program", "has_test_edits", "bit");
            await AssertColumnDefinition("program", "record_hash", "nvarchar");

            await AssertPrimaryKey("program", "program_id");
            await AssertUniqueIndex("program", "record_hash");
            await AssertIndex("program", "program_name");
            await AssertIndex("program", "folder_name");
            await AssertIndex("program", "task_name");
        }
    }

    [Test]
    public async Task RoutineTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "routine");

            await AssertColumnDefinition("routine", "routine_id", "bigint");
            await AssertColumnDefinition("routine", "container_name", "nvarchar");
            await AssertColumnDefinition("routine", "routine_name", "nvarchar");
            await AssertColumnDefinition("routine", "routine_description", "nvarchar");
            await AssertColumnDefinition("routine", "routine_type", "nvarchar");
            await AssertColumnDefinition("routine", "is_definition", "bit");
            await AssertColumnDefinition("routine", "record_hash", "nvarchar");

            await AssertPrimaryKey("routine", "routine_id");
            await AssertUniqueIndex("routine", "record_hash");
            await AssertIndex("routine", "container_name", "routine_name");
            await AssertIndex("routine", "routine_name");
        }
    }

    [Test]
    public async Task AoiTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "aoi");

            await AssertColumnDefinition("aoi", "aoi_id", "bigint");
            await AssertColumnDefinition("aoi", "aoi_name", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_description", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_revision", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_revision_extension", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_revision_note", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_vendor", "nvarchar");
            await AssertColumnDefinition("aoi", "aoi_help_text", "nvarchar");
            await AssertColumnDefinition("aoi", "created_date", "datetime");
            await AssertColumnDefinition("aoi", "created_by", "nvarchar");
            await AssertColumnDefinition("aoi", "edited_date", "datetime");
            await AssertColumnDefinition("aoi", "edited_by", "nvarchar");
            await AssertColumnDefinition("aoi", "software_revision", "nvarchar");
            await AssertColumnDefinition("aoi", "execute_pre_scan", "bit");
            await AssertColumnDefinition("aoi", "execute_post_scan", "bit");
            await AssertColumnDefinition("aoi", "execute_enable_in_false", "bit");
            await AssertColumnDefinition("aoi", "is_encrypted", "bit");
            await AssertColumnDefinition("aoi", "signature_id", "nvarchar");
            await AssertColumnDefinition("aoi", "signature_timestamp", "datetime");
            await AssertColumnDefinition("aoi", "component_class", "nvarchar");
            await AssertColumnDefinition("aoi", "content_hash", "nvarchar");
            await AssertColumnDefinition("aoi", "record_hash", "nvarchar");

            await AssertPrimaryKey("aoi", "aoi_id");
            await AssertUniqueIndex("aoi", "record_hash");
            await AssertIndex("aoi", "aoi_name");
        }
    }

    [Test]
    public async Task AoiParameterTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "aoi_parameter");

            await AssertColumnDefinition("aoi_parameter", "parameter_id", "bigint");
            await AssertColumnDefinition("aoi_parameter", "aoi_id", "bigint");
            await AssertColumnDefinition("aoi_parameter", "parameter_name", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "parameter_description", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "data_type", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "dimensions", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "radix", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "default_value", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "external_access", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "tag_usage", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "tag_type", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "tag_alias", "nvarchar");
            await AssertColumnDefinition("aoi_parameter", "is_visible", "bit");
            await AssertColumnDefinition("aoi_parameter", "is_required", "bit");
            await AssertColumnDefinition("aoi_parameter", "is_constant", "bit");
            await AssertColumnDefinition("aoi_parameter", "record_hash", "nvarchar");

            await AssertPrimaryKey("aoi_parameter", "parameter_id");
            await AssertUniqueIndex("aoi_parameter", "aoi_id", "record_hash");
            await AssertUniqueIndex("aoi_parameter", "aoi_id", "parameter_name");
            await AssertIndex("aoi_parameter", "parameter_name");
        }
    }

    [Test]
    public async Task TagTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag");

            await AssertColumnDefinition("tag", "tag_id", "bigint");
            await AssertColumnDefinition("tag", "program_name", "nvarchar");
            await AssertColumnDefinition("tag", "tag_name", "nvarchar");
            await AssertColumnDefinition("tag", "tag_description", "nvarchar");
            await AssertColumnDefinition("tag", "data_type", "nvarchar");
            await AssertColumnDefinition("tag", "dimensions", "nvarchar");
            await AssertColumnDefinition("tag", "radix", "nvarchar");
            await AssertColumnDefinition("tag", "external_access", "nvarchar");
            await AssertColumnDefinition("tag", "opcua_access", "nvarchar");
            await AssertColumnDefinition("tag", "is_constant", "bit");
            await AssertColumnDefinition("tag", "tag_usage", "nvarchar");
            await AssertColumnDefinition("tag", "tag_type", "nvarchar");
            await AssertColumnDefinition("tag", "alias_for", "nvarchar");
            await AssertColumnDefinition("tag", "content_hash", "nvarchar");
            await AssertColumnDefinition("tag", "record_hash", "nvarchar");

            await AssertPrimaryKey("tag", "tag_id");
            await AssertUniqueIndex("tag", "record_hash");
            await AssertIndex("tag", "program_name", "tag_name");
            await AssertIndex("tag", "tag_name");
            await AssertIndex("tag", "data_type");
        }
    }

    [Test]
    public async Task TagMemberTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_member");

            await AssertColumnDefinition("tag_member", "member_id", "bigint");
            await AssertColumnDefinition("tag_member", "tag_id", "bigint");
            await AssertColumnDefinition("tag_member", "member_path", "nvarchar");
            await AssertColumnDefinition("tag_member", "parent_path", "nvarchar");
            await AssertColumnDefinition("tag_member", "member_name", "nvarchar");
            await AssertColumnDefinition("tag_member", "data_type", "nvarchar");

            await AssertPrimaryKey("tag_member", "member_id");
            await AssertForeignKey("tag_member", "tag_id", "tag", "tag_id");
            await AssertUniqueIndex("tag_member", "tag_id", "member_path");
            await AssertIndex("tag_member", "member_path");
            await AssertIndex("tag_member", "parent_path", "tag_id");
            await AssertIndex("tag_member", "member_name", "tag_id");
            await AssertIndex("tag_member", "data_type", "tag_id");
        }
    }

    [Test]
    public async Task TagValueTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_value");

            await AssertColumnDefinition("tag_value", "version_id", "int");
            await AssertColumnDefinition("tag_value", "member_id", "bigint");
            await AssertColumnDefinition("tag_value", "tag_value", "nvarchar");

            await AssertForeignKey("tag_value", "version_id", "target_version", "version_id");
            await AssertForeignKey("tag_value", "member_id", "tag_member", "member_id");
        }
    }

    [Test]
    public async Task TagMemberCommentTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_member_comment");

            await AssertColumnDefinition("tag_member_comment", "tag_id", "bigint");
            await AssertColumnDefinition("tag_member_comment", "member_path", "nvarchar");
            await AssertColumnDefinition("tag_member_comment", "comment", "nvarchar");
            await AssertColumnDefinition("tag_member_comment", "record_hash", "nvarchar");

            await AssertForeignKey("tag_member_comment", "tag_id", "tag", "tag_id");
            await AssertUniqueIndex("tag_member_comment", "tag_id", "record_hash");
            await AssertUniqueIndex("tag_member_comment", "tag_id", "member_path");
            await AssertIndex("tag_member_comment", "member_path");
        }
    }

    [Test]
    public async Task TagProducerTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_producer");

            await AssertColumnDefinition("tag_producer", "tag_id", "bigint");
            await AssertColumnDefinition("tag_producer", "produce_count", "int");
            await AssertColumnDefinition("tag_producer", "send_event_trigger", "bit");
            await AssertColumnDefinition("tag_producer", "unicast_permitted", "bit");
            await AssertColumnDefinition("tag_producer", "maximum_rpi", "float");
            await AssertColumnDefinition("tag_producer", "minimum_rpi", "float");
            await AssertColumnDefinition("tag_producer", "default_rpi", "float");
            await AssertColumnDefinition("tag_producer", "record_hash", "nvarchar");

            await AssertForeignKey("tag_producer", "tag_id", "tag", "tag_id");
        }
    }

    [Test]
    public async Task TagConsumerTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "tag_consumer");

            await AssertColumnDefinition("tag_consumer", "tag_id", "bigint");
            await AssertColumnDefinition("tag_consumer", "producer", "nvarchar");
            await AssertColumnDefinition("tag_consumer", "remote_tag", "nvarchar");
            await AssertColumnDefinition("tag_consumer", "remote_instance", "int");
            await AssertColumnDefinition("tag_consumer", "rpi", "float");
            await AssertColumnDefinition("tag_consumer", "unicast", "bit");
            await AssertColumnDefinition("tag_consumer", "record_hash", "nvarchar");

            await AssertForeignKey("tag_consumer", "tag_id", "tag", "tag_id");
        }
    }

    [Test]
    public async Task RungTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "rung");

            await AssertColumnDefinition("rung", "rung_id", "bigint");
            await AssertColumnDefinition("rung", "container_name", "nvarchar");
            await AssertColumnDefinition("rung", "routine_name", "nvarchar");
            await AssertColumnDefinition("rung", "rung_number", "int");
            await AssertColumnDefinition("rung", "rung_text", "nvarchar");
            await AssertColumnDefinition("rung", "rung_comment", "nvarchar");
            await AssertColumnDefinition("rung", "code_hash", "nvarchar");
            await AssertColumnDefinition("rung", "record_hash", "nvarchar");

            await AssertPrimaryKey("rung", "rung_id");
            await AssertUniqueIndex("rung", "record_hash");
            await AssertIndex("rung", "container_name", "routine_name", "rung_number");
            await AssertIndex("rung", "routine_name", "rung_number");
            await AssertIndex("rung", "code_hash");
        }
    }

    [Test]
    public async Task RungInstructionTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "rung_instruction");

            await AssertColumnDefinition("rung_instruction", "rung_id", "bigint");
            await AssertColumnDefinition("rung_instruction", "instruction_index", "smallint");
            await AssertColumnDefinition("rung_instruction", "instruction_key", "nvarchar");
            await AssertColumnDefinition("rung_instruction", "instruction_text", "nvarchar");
            await AssertColumnDefinition("rung_instruction", "is_conditional", "bit");
            await AssertColumnDefinition("rung_instruction", "is_native", "bit");
            await AssertColumnDefinition("rung_instruction", "record_hash", "nvarchar");

            await AssertForeignKey("rung_instruction", "rung_id", "rung", "rung_id");
            await AssertUniqueIndex("rung_instruction", "rung_id", "instruction_index");
            await AssertIndex("rung_instruction", "instruction_key");
            await AssertIndex("rung_instruction", "record_hash");
        }
    }

    [Test]
    public async Task RungArgumentTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "rung_argument");

            await AssertColumnDefinition("rung_argument", "rung_id", "bigint");
            await AssertColumnDefinition("rung_argument", "instruction_index", "smallint");
            await AssertColumnDefinition("rung_argument", "argument_index", "tinyint");
            await AssertColumnDefinition("rung_argument", "argument_type", "nvarchar");
            await AssertColumnDefinition("rung_argument", "argument_text", "nvarchar");

            await AssertForeignKey("rung_argument", "rung_id", "rung", "rung_id");
            await AssertUniqueIndex("rung_argument", "rung_id", "instruction_index", "argument_index");
        }
    }

    [Test]
    public async Task RungReferenceTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "rung_reference");

            await AssertColumnDefinition("rung_reference", "rung_id", "bigint");
            await AssertColumnDefinition("rung_reference", "instruction_index", "smallint");
            await AssertColumnDefinition("rung_reference", "argument_index", "tinyint");
            await AssertColumnDefinition("rung_reference", "base_reference", "nvarchar");
            await AssertColumnDefinition("rung_reference", "member_reference", "nvarchar");

            await AssertForeignKey("rung_reference", "rung_id", "rung", "rung_id");
            await AssertIndex("rung_reference", "rung_id", "instruction_index", "argument_index");
            await AssertIndex("rung_reference", "base_reference");
        }
    }

    [Test]
    public async Task OperandTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "operand");

            await AssertColumnDefinition("operand", "operand_id", "bigint");
            await AssertColumnDefinition("operand", "instruction_key", "nvarchar");
            await AssertColumnDefinition("operand", "operand_index", "tinyint");
            await AssertColumnDefinition("operand", "operand_name", "nvarchar");
            await AssertColumnDefinition("operand", "operand_type", "nvarchar");
            await AssertColumnDefinition("operand", "operand_description", "nvarchar");
            await AssertColumnDefinition("operand", "is_destructive", "bit");
            await AssertColumnDefinition("operand", "is_native", "bit");
            await AssertColumnDefinition("operand", "record_hash", "nvarchar");

            await AssertPrimaryKey("operand", "operand_id");
            await AssertUniqueIndex("operand", "record_hash");
            await AssertIndex("operand", "instruction_key", "operand_index");
        }
    }

    [Test]
    public async Task OperandTable_PostMigration_ShouldHaveExpectedSeeds()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "operand");

            await AssertRecordExists("logix.operand", "instruction_key", "ABS");
            await AssertRecordExists("logix.operand", "instruction_key", "ALMA");
            await AssertRecordExists("logix.operand", "instruction_key", "MOVE");
            await AssertRecordExists("logix.operand", "instruction_key", "MOV");
            await AssertRecordExists("logix.operand", "instruction_key", "OTE");
            await AssertRecordExists("logix.operand", "instruction_key", "XIC");
            await AssertRecordExists("logix.operand", "instruction_key", "TON");
            await AssertRecordExists("logix.operand", "operand_name", "source");
            await AssertRecordExists("logix.operand", "operand_name", "destination");
        }
    }

    [Test]
    public async Task CoreVersionedFunctions_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("logix", "get_controller");
            await AssertFunctionExists("logix", "get_data_types");
            await AssertFunctionExists("logix", "get_aois");
            await AssertFunctionExists("logix", "get_modules");
            await AssertFunctionExists("logix", "get_tags");
            await AssertFunctionExists("logix", "get_tasks");
            await AssertFunctionExists("logix", "get_programs");
            await AssertFunctionExists("logix", "get_routines");
            await AssertFunctionExists("logix", "get_rungs");
            await AssertFunctionExists("logix", "get_logic");
        }
    }

    [Test]
    public async Task ModuleConnectionTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "module_connection");

            await AssertColumnDefinition("module_connection", "connection_id", "bigint");
            await AssertColumnDefinition("module_connection", "module_id", "bigint");
            await AssertColumnDefinition("module_connection", "connection_name", "nvarchar");
            await AssertColumnDefinition("module_connection", "rpi", "int");
            await AssertColumnDefinition("module_connection", "connection_type", "nvarchar");
            await AssertColumnDefinition("module_connection", "connection_priority", "nvarchar");
            await AssertColumnDefinition("module_connection", "transmission_type", "nvarchar");
            await AssertColumnDefinition("module_connection", "production_trigger", "nvarchar");
            await AssertColumnDefinition("module_connection", "output_redundant_owner", "bit");
            await AssertColumnDefinition("module_connection", "unicast", "bit");
            await AssertColumnDefinition("module_connection", "programatically_send_event_trigger", "bit");
            await AssertColumnDefinition("module_connection", "event_id", "int");
            await AssertColumnDefinition("module_connection", "input_tag", "nvarchar");
            await AssertColumnDefinition("module_connection", "input_size", "int");
            await AssertColumnDefinition("module_connection", "input_suffix", "nvarchar");
            await AssertColumnDefinition("module_connection", "output_tag", "nvarchar");
            await AssertColumnDefinition("module_connection", "output_size", "int");
            await AssertColumnDefinition("module_connection", "output_suffix", "nvarchar");
            await AssertColumnDefinition("module_connection", "connection_path", "nvarchar");
            await AssertColumnDefinition("module_connection", "record_hash", "nvarchar");

            await AssertPrimaryKey("module_connection", "connection_id");
            await AssertUniqueIndex("module_connection", "module_id", "record_hash");
        }
    }

    [Test]
    public async Task ModulePortTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "module_port");

            await AssertColumnDefinition("module_port", "port_id", "bigint");
            await AssertColumnDefinition("module_port", "module_id", "bigint");
            await AssertColumnDefinition("module_port", "port_number", "smallint");
            await AssertColumnDefinition("module_port", "port_type", "nvarchar");
            await AssertColumnDefinition("module_port", "address", "nvarchar");
            await AssertColumnDefinition("module_port", "upstream", "bit");
            await AssertColumnDefinition("module_port", "bus_size", "tinyint");
            await AssertColumnDefinition("module_port", "record_hash", "nvarchar");

            await AssertPrimaryKey("module_port", "port_id");
            await AssertUniqueIndex("module_port", "module_id", "record_hash");
            await AssertForeignKey("module_port", "module_id", "module", "module_id");
        }
    }

    [Test]
    public async Task GetTypeTreeFunction_PostMigration_ShouldHaveExpectedSchema()
    {
        await AssertFunctionExists("logix", "get_type_tree");
    }

    [Test]
    public async Task LatestVersionIdFunction_PostMigration_ShouldHaveExpectedSchema()
    {
        await AssertFunctionExists("logix", "latest_version_id");
    }

    [Test]
    public async Task ImportTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "import");
            await AssertColumnDefinition("import", "import_id", "uniqueidentifier");
            await AssertColumnDefinition("import", "import_status", "nvarchar");
            await AssertColumnDefinition("import", "source_type", "nvarchar");
            await AssertColumnDefinition("import", "file_type", "nvarchar");
            await AssertColumnDefinition("import", "file_name", "nvarchar");
            await AssertColumnDefinition("import", "posted_on", "datetime");
            await AssertPrimaryKey("import", "import_id");
        }
    }

    [Test]
    public async Task ImportLogTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("logix", "import_log");
            await AssertColumnDefinition("import_log", "log_id", "bigint");
            await AssertColumnDefinition("import_log", "import_id", "uniqueidentifier");
            await AssertColumnDefinition("import_log", "timestamp", "datetime");
            await AssertColumnDefinition("import_log", "log_severity", "nvarchar");
            await AssertColumnDefinition("import_log", "log_message", "nvarchar");
            await AssertColumnDefinition("import_log", "log_exception", "nvarchar");
            await AssertPrimaryKey("import_log", "log_id");
            await AssertForeignKey("import_log", "import_id", "import", "import_id");
            await AssertIndex("import_log", "import_id");
        }
    }

    [Test]
    public async Task LogixCommonFunctions_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertFunctionExists("logix", "tag_base");
            await AssertFunctionExists("logix", "tag_path");
            await AssertFunctionExists("logix", "is_atomic");
            await AssertFunctionExists("logix", "default_value");
        }
    }

    [Test]
    public async Task QaSchema_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertSchemaExists("qa");
            await AssertTypeExists("qa", "variables");
            await AssertTypeExists("qa", "validations");
            await AssertTypeExists("qa", "results");
        }
    }

    [Test]
    public async Task ValidationRunTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("qa", "validation_run");

            await AssertColumnDefinition("validation_run", "run_id", "bigint");
            await AssertColumnDefinition("validation_run", "run_name", "nvarchar");
            await AssertColumnDefinition("validation_run", "run_status", "nvarchar");
            await AssertColumnDefinition("validation_run", "executed_by", "nvarchar");
            await AssertColumnDefinition("validation_run", "executed_on", "datetime2");
            await AssertColumnDefinition("validation_run", "completed_on", "datetime2");
            await AssertColumnDefinition("validation_run", "variables_data", "nvarchar");
            await AssertColumnDefinition("validation_run", "variables_hash", "varbinary");

            await AssertPrimaryKey("validation_run", "run_id");
            await AssertIndex("validation_run", "variables_hash");
        }
    }

    [Test]
    public async Task ValidationResultTable_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertTableExists("qa", "validation_result");

            await AssertColumnDefinition("validation_result", "result_id", "bigint");
            await AssertColumnDefinition("validation_result", "run_id", "bigint");
            await AssertColumnDefinition("validation_result", "validation_name", "nvarchar");
            await AssertColumnDefinition("validation_result", "execution_time", "datetime2");
            await AssertColumnDefinition("validation_result", "is_success", "bit");
            await AssertColumnDefinition("validation_result", "result_message", "nvarchar");
            await AssertColumnDefinition("validation_result", "result_details", "nvarchar");

            await AssertPrimaryKey("validation_result", "result_id");
            await AssertForeignKey("validation_result", "run_id", "validation_run", "run_id");
            await AssertIndex("validation_result", "run_id", "validation_name");
            await AssertIndex("validation_result", "validation_name");
        }
    }

    [Test]
    public async Task QaSchemaObjects_PostMigration_ShouldHaveExpectedSchema()
    {
        using (Assert.EnterMultipleScope())
        {
            await AssertViewExists("qa", "list_validations");

            await AssertFunctionExists("qa", "emit_failure");
            await AssertFunctionExists("qa", "emit_success");
            await AssertFunctionExists("qa", "hash");

            await AssertProcedureExists("qa", "create_validation");
            await AssertProcedureExists("qa", "get_variable");
            await AssertProcedureExists("qa", "get_variable_as_int");
            await AssertProcedureExists("qa", "run_validations");
            await AssertProcedureExists("qa", "get_variable_as_bit");
            await AssertProcedureExists("qa", "get_variable_as_real");
            await AssertProcedureExists("qa", "get_variable_as_date");
            await AssertProcedureExists("qa", "rerun_validations");
            await AssertProcedureExists("qa", "generate_approval");
            await AssertProcedureExists("qa", "execute_validation");
            await AssertProcedureExists("qa", "get_override_variable");
            await AssertProcedureExists("qa", "inspect_result");
            await AssertProcedureExists("qa", "create_suite");
        }
    }
}