using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260308;

/// <summary>
/// Represents a FluentMigrator migration that seeds the initial set of operands into the database.
/// </summary>
[UsedImplicitly]
[Migration(202603082130, "Seeds initial operand metadata for all supported Logix instructions")]
public class M017SeedInitialOperands : AutoReversingMigration
{
    public override void Up()
    {
        var insert = Insert.IntoTable("operand");

        insert.Row(new
        {
            instruction_key = "ABL",
            operand_index = 0,
            operand_name = "channel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ABL",
            operand_index = 1,
            operand_name = "serial_port_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ABL",
            operand_index = 2,
            operand_name = "character_count",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ABS",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ABS",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ACB",
            operand_index = 0,
            operand_name = "channel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ACB",
            operand_index = 1,
            operand_name = "serial_port_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ACB",
            operand_index = 2,
            operand_name = "character_count",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ACL",
            operand_index = 0,
            operand_name = "channel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ACL",
            operand_index = 1,
            operand_name = "clear_serial_port_read",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ACL",
            operand_index = 2,
            operand_name = "clear_serial_port_write",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ACS",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ACS",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ACOS",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ACOS",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ADD",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ADD",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ADD",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "AHL",
            operand_index = 0,
            operand_name = "channel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AHL",
            operand_index = 1,
            operand_name = "ANDMask",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AHL",
            operand_index = 2,
            operand_name = "ORMask",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AHL",
            operand_index = 3,
            operand_name = "serial_port_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "AHL",
            operand_index = 4,
            operand_name = "channel_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ALMA",
            operand_index = 0,
            operand_name = "alma_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ALMA",
            operand_index = 1,
            operand_name = "in",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ALMA",
            operand_index = 2,
            operand_name = "program_acknowledge_all",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ALMA",
            operand_index = 3,
            operand_name = "program_disable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ALMA",
            operand_index = 4,
            operand_name = "program_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ALMD",
            operand_index = 0,
            operand_name = "almd_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ALMD",
            operand_index = 1,
            operand_name = "program_acknowledge",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ALMD",
            operand_index = 2,
            operand_name = "program_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ALMD",
            operand_index = 3,
            operand_name = "program_disable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ALMD",
            operand_index = 4,
            operand_name = "program_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AND",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AND",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AND",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ARD",
            operand_index = 0,
            operand_name = "channel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ARD",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ARD",
            operand_index = 2,
            operand_name = "serial_port_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ARD",
            operand_index = 3,
            operand_name = "string_length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ARD",
            operand_index = 4,
            operand_name = "characters_read",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ARL",
            operand_index = 0,
            operand_name = "channel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ARL",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ARL",
            operand_index = 2,
            operand_name = "serial_port_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ARL",
            operand_index = 3,
            operand_name = "string_length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ARL",
            operand_index = 4,
            operand_name = "characters_read",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ASN",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ASN",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ASIN",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ASIN",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ATN",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ATN",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ATAN",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ATAN",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 0,
            operand_name = "avc_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 1,
            operand_name = "feedback_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 2,
            operand_name = "feedback_reation_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 3,
            operand_name = "delay_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 4,
            operand_name = "delay_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 5,
            operand_name = "output_follows_actuate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 6,
            operand_name = "actuate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 7,
            operand_name = "delay_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 8,
            operand_name = "feedback_1",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 9,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 10,
            operand_name = "output_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVC",
            operand_index = 11,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVE",
            operand_index = 0,
            operand_name = "array",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVE",
            operand_index = 1,
            operand_name = "dim_to_vary",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVE",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "AVE",
            operand_index = 3,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "AVE",
            operand_index = 4,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AVE",
            operand_index = 5,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AWA",
            operand_index = 0,
            operand_name = "channel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AWA",
            operand_index = 1,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AWA",
            operand_index = 2,
            operand_name = "serial_port_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "AWA",
            operand_index = 3,
            operand_name = "string_length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AWA",
            operand_index = 4,
            operand_name = "characters_sent",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AWT",
            operand_index = 0,
            operand_name = "channel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AWT",
            operand_index = 1,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AWT",
            operand_index = 2,
            operand_name = "serial_port_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "AWT",
            operand_index = 3,
            operand_name = "string_length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "AWT",
            operand_index = 4,
            operand_name = "characters_sent",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BSL",
            operand_index = 0,
            operand_name = "array",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BSL",
            operand_index = 1,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "BSL",
            operand_index = 2,
            operand_name = "source_bit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BSL",
            operand_index = 3,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BSR",
            operand_index = 0,
            operand_name = "array",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BSR",
            operand_index = 1,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "BSR",
            operand_index = 2,
            operand_name = "source_bit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BSR",
            operand_index = 3,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BTD",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BTD",
            operand_index = 1,
            operand_name = "source_bit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BTD",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "BTD",
            operand_index = 3,
            operand_name = "destination_bit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "BTD",
            operand_index = 4,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 0,
            operand_name = "cbcm_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 1,
            operand_name = "ack_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 2,
            operand_name = "mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 3,
            operand_name = "takeover_mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 4,
            operand_name = "enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 5,
            operand_name = "safety_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 6,
            operand_name = "standard_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 7,
            operand_name = "arm_continuous",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 8,
            operand_name = "start",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 9,
            operand_name = "stop_at_top",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 10,
            operand_name = "press_in_motion",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 11,
            operand_name = "motion_monitor_fault",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 12,
            operand_name = "slide_zone",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBCM",
            operand_index = 13,
            operand_name = "safety_enable_ack",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 0,
            operand_name = "cbim_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 1,
            operand_name = "ack_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 2,
            operand_name = "inch_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 3,
            operand_name = "enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 4,
            operand_name = "safety_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 5,
            operand_name = "standard_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 6,
            operand_name = "start",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 7,
            operand_name = "press_in_motion",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 8,
            operand_name = "motion_monitor_fault",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 9,
            operand_name = "slide_zone",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBIM",
            operand_index = 10,
            operand_name = "safety_enable_ack",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 0,
            operand_name = "cbssm_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 1,
            operand_name = "ack_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 2,
            operand_name = "takeover_mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 3,
            operand_name = "enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 4,
            operand_name = "safety_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 5,
            operand_name = "standard_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 6,
            operand_name = "start",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 7,
            operand_name = "press_in_motion",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 8,
            operand_name = "motion_monitor_fault",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 9,
            operand_name = "slide_zone",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CBSSM",
            operand_index = 10,
            operand_name = "saefty_enable_ack",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CLR",
            operand_index = 0,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CMP",
            operand_index = 0,
            operand_name = "expression",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CONCAT",
            operand_index = 0,
            operand_name = "sourceA",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CONCAT",
            operand_index = 1,
            operand_name = "sourceB",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CONCAT",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "COP",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "COP",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "COP",
            operand_index = 2,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "COS",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "COS",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 0,
            operand_name = "cpm_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 1,
            operand_name = "cam_profile",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 2,
            operand_name = "enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 3,
            operand_name = "brake_cam",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 4,
            operand_name = "takeover_cam",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 5,
            operand_name = "dynamic_cam",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 6,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 7,
            operand_name = "reverse",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 8,
            operand_name = "press_motion_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPM",
            operand_index = 9,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPS",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPS",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CPS",
            operand_index = 2,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CPT",
            operand_index = 0,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CPT",
            operand_index = 1,
            operand_name = "expression",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CROUT",
            operand_index = 0,
            operand_name = "crout_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CROUT",
            operand_index = 1,
            operand_name = "feedback_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CROUT",
            operand_index = 2,
            operand_name = "feedback_reaction_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CROUT",
            operand_index = 3,
            operand_name = "actuate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CROUT",
            operand_index = 4,
            operand_name = "feedback_1",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CROUT",
            operand_index = 5,
            operand_name = "feedback_2",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CROUT",
            operand_index = 6,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CROUT",
            operand_index = 7,
            operand_name = "output_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CROUT",
            operand_index = 8,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CSM",
            operand_index = 0,
            operand_name = "csm_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "CSM",
            operand_index = 1,
            operand_name = "mechanical_delay_timer",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CSM",
            operand_index = 2,
            operand_name = "max_pulse_period",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CSM",
            operand_index = 3,
            operand_name = "motion_request",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CSM",
            operand_index = 4,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CSM",
            operand_index = 5,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CSM",
            operand_index = 6,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CSM",
            operand_index = 7,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CTD",
            operand_index = 0,
            operand_name = "counter",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CTD",
            operand_index = 1,
            operand_name = "preset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CTD",
            operand_index = 2,
            operand_name = "accum",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CTU",
            operand_index = 0,
            operand_name = "counter",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CTU",
            operand_index = 1,
            operand_name = "preset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "CTU",
            operand_index = 2,
            operand_name = "accum",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCM",
            operand_index = 0,
            operand_name = "dcm_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DCM",
            operand_index = 1,
            operand_name = "safety_function",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCM",
            operand_index = 2,
            operand_name = "input_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCM",
            operand_index = 3,
            operand_name = "descrepancy_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCM",
            operand_index = 4,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCM",
            operand_index = 5,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCM",
            operand_index = 6,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCM",
            operand_index = 7,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 0,
            operand_name = "dcs_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 1,
            operand_name = "safety_function",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 2,
            operand_name = "input_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 3,
            operand_name = "discrepancy_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 4,
            operand_name = "restart_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 5,
            operand_name = "cold_start_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 6,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 7,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 8,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCS",
            operand_index = 9,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSRT",
            operand_index = 0,
            operand_name = "dcsrt_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DCSRT",
            operand_index = 1,
            operand_name = "safety_function",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSRT",
            operand_index = 2,
            operand_name = "input_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSRT",
            operand_index = 3,
            operand_name = "discrepancy_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSRT",
            operand_index = 4,
            operand_name = "enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSRT",
            operand_index = 5,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSRT",
            operand_index = 6,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSRT",
            operand_index = 7,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSRT",
            operand_index = 8,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 0,
            operand_name = "dcst_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 1,
            operand_name = "safety_function",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 2,
            operand_name = "input_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 3,
            operand_name = "discrepancy_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 4,
            operand_name = "restart_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 5,
            operand_name = "cold_start_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 6,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 7,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 8,
            operand_name = "test_request",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 9,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCST",
            operand_index = 10,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 0,
            operand_name = "dcstm_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 1,
            operand_name = "safety_function",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 2,
            operand_name = "input_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 3,
            operand_name = "discrepancy_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 4,
            operand_name = "restart_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 5,
            operand_name = "cold_start_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 6,
            operand_name = "test_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 7,
            operand_name = "test_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 8,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 9,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 10,
            operand_name = "test_request",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 11,
            operand_name = "mute",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 12,
            operand_name = "muting_lamp_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 13,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTM",
            operand_index = 14,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 0,
            operand_name = "dcstl_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 1,
            operand_name = "safety_function",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 2,
            operand_name = "input_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 3,
            operand_name = "discrepancy_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 4,
            operand_name = "restart_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 5,
            operand_name = "cold_start_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 6,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 7,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 8,
            operand_name = "test_request",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 9,
            operand_name = "unlock_request",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 10,
            operand_name = "lock_feedback",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 11,
            operand_name = "hazard_stopped",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 12,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DCSTL",
            operand_index = 13,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DDT",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DDT",
            operand_index = 1,
            operand_name = "reference",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DDT",
            operand_index = 2,
            operand_name = "result",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DDT",
            operand_index = 3,
            operand_name = "cmp_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DDT",
            operand_index = 4,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DDT",
            operand_index = 5,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DDT",
            operand_index = 6,
            operand_name = "result_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DDT",
            operand_index = 7,
            operand_name = "result_length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DDT",
            operand_index = 8,
            operand_name = "result_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DEG",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DEG",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DELETE",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DELETE",
            operand_index = 1,
            operand_name = "quantity",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DELETE",
            operand_index = 2,
            operand_name = "start",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DELETE",
            operand_index = 3,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DIN",
            operand_index = 0,
            operand_name = "din_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DIN",
            operand_index = 1,
            operand_name = "reset_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DIN",
            operand_index = 2,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DIN",
            operand_index = 3,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DIN",
            operand_index = 4,
            operand_name = "circuit_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DIN",
            operand_index = 5,
            operand_name = "fault_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DIV",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DIV",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DIV",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DTOS",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DTOS",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "DTR",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DTR",
            operand_index = 1,
            operand_name = "mask",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "DTR",
            operand_index = 2,
            operand_name = "reference",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ENPEN",
            operand_index = 0,
            operand_name = "enpen_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ENPEN",
            operand_index = 1,
            operand_name = "reset_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ENPEN",
            operand_index = 2,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ENPEN",
            operand_index = 3,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ENPEN",
            operand_index = 4,
            operand_name = "circuit_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ENPEN",
            operand_index = 5,
            operand_name = "fault_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EOT",
            operand_index = 0,
            operand_name = "data_bit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 0,
            operand_name = "epms_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 1,
            operand_name = "input_1",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 2,
            operand_name = "input_2",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 3,
            operand_name = "input_3",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 4,
            operand_name = "input_4",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 5,
            operand_name = "input_5",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 6,
            operand_name = "input_6",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 7,
            operand_name = "input_7",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 8,
            operand_name = "input_8",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 9,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 10,
            operand_name = "lck",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EPMS",
            operand_index = 11,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EQU",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EQU",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EQ",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EQ",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ESTOP",
            operand_index = 0,
            operand_name = "estop_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ESTOP",
            operand_index = 1,
            operand_name = "reset_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ESTOP",
            operand_index = 2,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ESTOP",
            operand_index = 3,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ESTOP",
            operand_index = 4,
            operand_name = "circuit_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ESTOP",
            operand_index = 5,
            operand_name = "fault_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EVENT",
            operand_index = 0,
            operand_name = "task",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FAL",
            operand_index = 0,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FAL",
            operand_index = 1,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FAL",
            operand_index = 2,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FAL",
            operand_index = 3,
            operand_name = "mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FAL",
            operand_index = 4,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FAL",
            operand_index = 5,
            operand_name = "expression",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FBC",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FBC",
            operand_index = 1,
            operand_name = "reference",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FBC",
            operand_index = 2,
            operand_name = "result",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FBC",
            operand_index = 3,
            operand_name = "cmp_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FBC",
            operand_index = 4,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FBC",
            operand_index = 5,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FBC",
            operand_index = 6,
            operand_name = "result_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FBC",
            operand_index = 7,
            operand_name = "result_length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FBC",
            operand_index = 8,
            operand_name = "result_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FFL",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FFL",
            operand_index = 1,
            operand_name = "FIFO",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FFL",
            operand_index = 2,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FFL",
            operand_index = 3,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FFL",
            operand_index = 4,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FFU",
            operand_index = 0,
            operand_name = "FIFO",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FFU",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FFU",
            operand_index = 2,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FFU",
            operand_index = 3,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FFU",
            operand_index = 4,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FIND",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FIND",
            operand_index = 1,
            operand_name = "search",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FIND",
            operand_index = 2,
            operand_name = "start",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FIND",
            operand_index = 3,
            operand_name = "result",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FLL",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FLL",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FLL",
            operand_index = 2,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FOR",
            operand_index = 0,
            operand_name = "routine_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FOR",
            operand_index = 1,
            operand_name = "index",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FOR",
            operand_index = 2,
            operand_name = "initial_value",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FOR",
            operand_index = 3,
            operand_name = "terminal_value",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FOR",
            operand_index = 4,
            operand_name = "step_size",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FPMS",
            operand_index = 0,
            operand_name = "fpms_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FPMS",
            operand_index = 1,
            operand_name = "input_1",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FPMS",
            operand_index = 2,
            operand_name = "input_2",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FPMS",
            operand_index = 3,
            operand_name = "input_3",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FPMS",
            operand_index = 4,
            operand_name = "input_4",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FPMS",
            operand_index = 5,
            operand_name = "input_5",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FPMS",
            operand_index = 6,
            operand_name = "fault_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FRD",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FRD",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 0,
            operand_name = "fsbm_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 1,
            operand_name = "restart_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 2,
            operand_name = "S1_S2_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 3,
            operand_name = "S2_LC_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 4,
            operand_name = "LC_S3_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 5,
            operand_name = "S3_S4_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 6,
            operand_name = "maximum_mute_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 7,
            operand_name = "maximum_override_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 8,
            operand_name = "direction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 9,
            operand_name = "light_curtain",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 10,
            operand_name = "sensor_1",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 11,
            operand_name = "sensor_2",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 12,
            operand_name = "sensor_3",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 13,
            operand_name = "sensor_4",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 14,
            operand_name = "enable_mute",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 15,
            operand_name = "override",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 16,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 17,
            operand_name = "muting_lamp_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSBM",
            operand_index = 18,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSC",
            operand_index = 0,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "FSC",
            operand_index = 1,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSC",
            operand_index = 2,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSC",
            operand_index = 3,
            operand_name = "mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "FSC",
            operand_index = 4,
            operand_name = "expression",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GEQ",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GEQ",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GE",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GE",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GRT",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GRT",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GT",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GT",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GSV",
            operand_index = 0,
            operand_name = "class_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GSV",
            operand_index = 1,
            operand_name = "instance_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GSV",
            operand_index = 2,
            operand_name = "attribute_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "GSV",
            operand_index = 3,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "INSERT",
            operand_index = 0,
            operand_name = "sourceA",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "INSERT",
            operand_index = 1,
            operand_name = "sourceB",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "INSERT",
            operand_index = 2,
            operand_name = "start",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "INSERT",
            operand_index = 3,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "IOT",
            operand_index = 0,
            operand_name = "output_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "JMP",
            operand_index = 0,
            operand_name = "label_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "JSR",
            operand_index = 0,
            operand_name = "routine_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "JSR",
            operand_index = 1,
            operand_name = "number_of_inputs",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "JSR",
            operand_index = 2,
            operand_name = "parameters",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "JXR",
            operand_index = 0,
            operand_name = "external_routine_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "JXR",
            operand_index = 1,
            operand_name = "external_routine_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "JXR",
            operand_index = 2,
            operand_name = "parameter",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "JXR",
            operand_index = 3,
            operand_name = "return_parameter",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LBL",
            operand_index = 0,
            operand_name = "label_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LC",
            operand_index = 0,
            operand_name = "lc_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "LC",
            operand_index = 1,
            operand_name = "reset_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LC",
            operand_index = 2,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LC",
            operand_index = 3,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LC",
            operand_index = 4,
            operand_name = "input_filter_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LC",
            operand_index = 5,
            operand_name = "mute_light_curtain",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LC",
            operand_index = 6,
            operand_name = "circuit_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LC",
            operand_index = 7,
            operand_name = "fault_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LEQ",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LEQ",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LE",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LE",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LES",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LES",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LT",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LT",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LFL",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LFL",
            operand_index = 1,
            operand_name = "LIFO",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LFL",
            operand_index = 2,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "LFL",
            operand_index = 3,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LFL",
            operand_index = 4,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LFU",
            operand_index = 0,
            operand_name = "LIFO",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LFU",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "LFU",
            operand_index = 2,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "LFU",
            operand_index = 3,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LFU",
            operand_index = 4,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LIM",
            operand_index = 0,
            operand_name = "low_limit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LIM",
            operand_index = 1,
            operand_name = "test",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LIM",
            operand_index = 2,
            operand_name = "high_limit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LIMIT",
            operand_index = 0,
            operand_name = "low_limit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LIMIT",
            operand_index = 1,
            operand_name = "test",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LIMIT",
            operand_index = 2,
            operand_name = "high_limit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LN",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LN",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "LOG",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LOG",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "LOWER",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "LOWER",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAAT",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAAT",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAFR",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAFR",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 0,
            operand_name = "slave_axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 1,
            operand_name = "master_axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 2,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 3,
            operand_name = "direction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 4,
            operand_name = "ratio",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 5,
            operand_name = "slave_counts",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 6,
            operand_name = "master_counts",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 7,
            operand_name = "master_reference",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 8,
            operand_name = "ratio_format",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 9,
            operand_name = "clutch",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 10,
            operand_name = "accel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAG",
            operand_index = 11,
            operand_name = "accel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAH",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAH",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAHD",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAHD",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAHD",
            operand_index = 2,
            operand_name = "diagnostic_test",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAHD",
            operand_index = 3,
            operand_name = "observed_direction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 2,
            operand_name = "direction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 3,
            operand_name = "speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 4,
            operand_name = "speed_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 5,
            operand_name = "accel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 6,
            operand_name = "accel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 7,
            operand_name = "decel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 8,
            operand_name = "decel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 9,
            operand_name = "profile",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 10,
            operand_name = "merge",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAJ",
            operand_index = 11,
            operand_name = "merge_speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 2,
            operand_name = "move_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 3,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 4,
            operand_name = "speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 5,
            operand_name = "speed_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 6,
            operand_name = "accel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 7,
            operand_name = "accel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 8,
            operand_name = "decel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 9,
            operand_name = "decel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 10,
            operand_name = "profile",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 11,
            operand_name = "merge",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAM",
            operand_index = 12,
            operand_name = "merge_speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 1,
            operand_name = "execution_target",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 2,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 3,
            operand_name = "output",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 4,
            operand_name = "input",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 5,
            operand_name = "output_cam",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 6,
            operand_name = "cam_start_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 7,
            operand_name = "cam_end_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 8,
            operand_name = "output_compensation",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 9,
            operand_name = "execution_mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 10,
            operand_name = "execution_schedule",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 11,
            operand_name = "axis_arm_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 12,
            operand_name = "cam_arm_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAOC",
            operand_index = 13,
            operand_name = "reference",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 0,
            operand_name = "slave_axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 1,
            operand_name = "master_axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 2,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 3,
            operand_name = "direction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 4,
            operand_name = "cam_profile",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 5,
            operand_name = "slave_scaling",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 6,
            operand_name = "master_scaling",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 7,
            operand_name = "execution_mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 8,
            operand_name = "execution_schedule",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 9,
            operand_name = "master_lock_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 10,
            operand_name = "cam_lock_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 11,
            operand_name = "master_reference",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAPC",
            operand_index = 12,
            operand_name = "master_direction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAR",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAR",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAR",
            operand_index = 2,
            operand_name = "trigger_condition",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAR",
            operand_index = 3,
            operand_name = "windowed_registration",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAR",
            operand_index = 4,
            operand_name = "minimum_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAR",
            operand_index = 5,
            operand_name = "maximum_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAS",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAS",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAS",
            operand_index = 2,
            operand_name = "stop_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAS",
            operand_index = 3,
            operand_name = "change_decel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAS",
            operand_index = 4,
            operand_name = "decel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAS",
            operand_index = 5,
            operand_name = "decel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MASD",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MASD",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MASR",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MASR",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MATC",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MATC",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MATC",
            operand_index = 2,
            operand_name = "direction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MATC",
            operand_index = 3,
            operand_name = "cam_profile",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MATC",
            operand_index = 4,
            operand_name = "distance_scaling",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MATC",
            operand_index = 5,
            operand_name = "time_scaling",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MATC",
            operand_index = 6,
            operand_name = "execution_mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MATC",
            operand_index = 7,
            operand_name = "execution_schedule",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAW",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAW",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MAW",
            operand_index = 2,
            operand_name = "trigger_condition",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MAW",
            operand_index = 3,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 0,
            operand_name = "coordinate_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 2,
            operand_name = "motion_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 3,
            operand_name = "change_speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 4,
            operand_name = "speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 5,
            operand_name = "speed_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 6,
            operand_name = "change_accel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 7,
            operand_name = "accel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 8,
            operand_name = "accel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 9,
            operand_name = "change_decel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 10,
            operand_name = "decel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 11,
            operand_name = "decel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCD",
            operand_index = 12,
            operand_name = "scope",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 0,
            operand_name = "coordinate_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 2,
            operand_name = "move_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 3,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 4,
            operand_name = "circle_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 5,
            operand_name = "radius",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 6,
            operand_name = "direction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 7,
            operand_name = "speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 8,
            operand_name = "speed_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 9,
            operand_name = "accel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 10,
            operand_name = "accel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 11,
            operand_name = "decel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 12,
            operand_name = "decel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 13,
            operand_name = "profile",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 14,
            operand_name = "termination_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 15,
            operand_name = "merge",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCM",
            operand_index = 16,
            operand_name = "merge_speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCP",
            operand_index = 0,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCCP",
            operand_index = 1,
            operand_name = "cam",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCP",
            operand_index = 2,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCP",
            operand_index = 3,
            operand_name = "start_slope",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCP",
            operand_index = 4,
            operand_name = "end_slope",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCCP",
            operand_index = 5,
            operand_name = "cam_profile",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 0,
            operand_name = "coordinate_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 2,
            operand_name = "move_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 3,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 4,
            operand_name = "speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 5,
            operand_name = "speed_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 6,
            operand_name = "accel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 7,
            operand_name = "accel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 8,
            operand_name = "decel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 9,
            operand_name = "decel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 10,
            operand_name = "profile",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 11,
            operand_name = "termination_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 12,
            operand_name = "merge",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCLM",
            operand_index = 13,
            operand_name = "merge_speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 2,
            operand_name = "motion_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 3,
            operand_name = "change_speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 4,
            operand_name = "speed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 5,
            operand_name = "change_accel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 6,
            operand_name = "accel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 7,
            operand_name = "change_decel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 8,
            operand_name = "decel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 9,
            operand_name = "speed_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 10,
            operand_name = "accel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCD",
            operand_index = 11,
            operand_name = "decel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCS",
            operand_index = 0,
            operand_name = "coordinate_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCS",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCS",
            operand_index = 2,
            operand_name = "stop_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCS",
            operand_index = 3,
            operand_name = "change_decel",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCS",
            operand_index = 4,
            operand_name = "decel_rate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCS",
            operand_index = 5,
            operand_name = "decel_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCSD",
            operand_index = 0,
            operand_name = "coordinate_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCSD",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCSR",
            operand_index = 0,
            operand_name = "coordinate_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCSR",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCSV",
            operand_index = 0,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCSV",
            operand_index = 1,
            operand_name = "cam_profile",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCSV",
            operand_index = 2,
            operand_name = "master_value",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCSV",
            operand_index = 3,
            operand_name = "slave_value",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCSV",
            operand_index = 4,
            operand_name = "slope_value",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCSV",
            operand_index = 5,
            operand_name = "slope_derivative",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCT",
            operand_index = 0,
            operand_name = "source_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCT",
            operand_index = 1,
            operand_name = "target_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCT",
            operand_index = 2,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCT",
            operand_index = 3,
            operand_name = "orientation",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCT",
            operand_index = 4,
            operand_name = "translation",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCTP",
            operand_index = 0,
            operand_name = "source_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCTP",
            operand_index = 1,
            operand_name = "target_system",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCTP",
            operand_index = 2,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MCTP",
            operand_index = 3,
            operand_name = "orientation",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCTP",
            operand_index = 4,
            operand_name = "translation",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCTP",
            operand_index = 5,
            operand_name = "transform_direction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCTP",
            operand_index = 6,
            operand_name = "reference_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MCTP",
            operand_index = 7,
            operand_name = "transform_position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDF",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDF",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MDO",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDO",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MDO",
            operand_index = 2,
            operand_name = "drive_output",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDO",
            operand_index = 3,
            operand_name = "drive_units",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDOC",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDOC",
            operand_index = 1,
            operand_name = "execution_target",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDOC",
            operand_index = 2,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MDOC",
            operand_index = 3,
            operand_name = "disarm_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDR",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDR",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MDW",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MDW",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MEQ",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MEQ",
            operand_index = 1,
            operand_name = "mask",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MEQ",
            operand_index = 2,
            operand_name = "compare",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MGS",
            operand_index = 0,
            operand_name = "group",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MGS",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MGS",
            operand_index = 2,
            operand_name = "stop_mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MGSD",
            operand_index = 0,
            operand_name = "group",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MGSD",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MGSP",
            operand_index = 0,
            operand_name = "group",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MGSP",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MGSR",
            operand_index = 0,
            operand_name = "group",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MGSR",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MID",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MID",
            operand_index = 1,
            operand_name = "quantity",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MID",
            operand_index = 2,
            operand_name = "start",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MID",
            operand_index = 3,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 0,
            operand_name = "mmvc_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 1,
            operand_name = "enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 2,
            operand_name = "keyswitch",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 3,
            operand_name = "bottom",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 4,
            operand_name = "flywheel_stopped",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 5,
            operand_name = "safety_enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 6,
            operand_name = "actuate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 7,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 8,
            operand_name = "output_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MMVC",
            operand_index = 9,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MOD",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MOD",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MOD",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MOV",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MOV",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MOVE",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MOVE",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MRAT",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MRAT",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MRHD",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MRHD",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MRHD",
            operand_index = 2,
            operand_name = "diagnostic_test",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MRP",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MRP",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MRP",
            operand_index = 2,
            operand_name = "type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MRP",
            operand_index = 3,
            operand_name = "position_select",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MRP",
            operand_index = 4,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MSF",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MSF",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MSG",
            operand_index = 0,
            operand_name = "message_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MSO",
            operand_index = 0,
            operand_name = "axis",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MSO",
            operand_index = 1,
            operand_name = "motion_control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MUL",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MUL",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MUL",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MVC",
            operand_index = 0,
            operand_name = "mvc_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "MVC",
            operand_index = 1,
            operand_name = "feedback_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVC",
            operand_index = 2,
            operand_name = "feedback_reaction_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVC",
            operand_index = 3,
            operand_name = "actuate",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVC",
            operand_index = 4,
            operand_name = "feedback_1",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVC",
            operand_index = 5,
            operand_name = "feedback_2",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVC",
            operand_index = 6,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVC",
            operand_index = 7,
            operand_name = "output_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVC",
            operand_index = 8,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVM",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVM",
            operand_index = 1,
            operand_name = "mask",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "MVM",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "NEG",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "NEG",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "NEQ",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "NEQ",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "NE",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "NE",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "NOT",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "NOT",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ONS",
            operand_index = 0,
            operand_name = "storage_bit",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "OR",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "OR",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "OR",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "OSF",
            operand_index = 0,
            operand_name = "storage_bit",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "OSF",
            operand_index = 1,
            operand_name = "output_bit",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "OSR",
            operand_index = 0,
            operand_name = "storage_bit",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "OSR",
            operand_index = 1,
            operand_name = "output_bit",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "OTE",
            operand_index = 0,
            operand_name = "data_bit",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "OTL",
            operand_index = 0,
            operand_name = "data_bit",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "OTU",
            operand_index = 0,
            operand_name = "data_bit",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "PATT",
            operand_index = 0,
            operand_name = "phase_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PATT",
            operand_index = 1,
            operand_name = "result",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PCLF",
            operand_index = 0,
            operand_name = "phase_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PCMD",
            operand_index = 0,
            operand_name = "phase_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PCMD",
            operand_index = 1,
            operand_name = "command",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PCMD",
            operand_index = 2,
            operand_name = "result",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PDET",
            operand_index = 0,
            operand_name = "phase_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PFL",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PID",
            operand_index = 0,
            operand_name = "PID",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PID",
            operand_index = 1,
            operand_name = "process_variable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PID",
            operand_index = 2,
            operand_name = "tieback",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PID",
            operand_index = 3,
            operand_name = "control_variable",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "PID",
            operand_index = 4,
            operand_name = "pid_master_loop",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PID",
            operand_index = 5,
            operand_name = "inhold_bit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PID",
            operand_index = 6,
            operand_name = "inhold_value",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "POVR",
            operand_index = 0,
            operand_name = "phase_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "POVR",
            operand_index = 1,
            operand_name = "command",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "POVR",
            operand_index = 2,
            operand_name = "result",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PXRQ",
            operand_index = 0,
            operand_name = "phase_instruction",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PXRQ",
            operand_index = 1,
            operand_name = "external_request",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "PXRQ",
            operand_index = 2,
            operand_name = "data_value",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RAD",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RAD",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "RES",
            operand_index = 0,
            operand_name = "structure",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RET",
            operand_index = 0,
            operand_name = "outputs",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RIN",
            operand_index = 0,
            operand_name = "rin_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "RIN",
            operand_index = 1,
            operand_name = "reset_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RIN",
            operand_index = 2,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RIN",
            operand_index = 3,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RIN",
            operand_index = 4,
            operand_name = "circuit_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RIN",
            operand_index = 5,
            operand_name = "fault_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ROUT",
            operand_index = 0,
            operand_name = "rout_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "ROUT",
            operand_index = 1,
            operand_name = "feedback_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ROUT",
            operand_index = 2,
            operand_name = "enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ROUT",
            operand_index = 3,
            operand_name = "feedback_1",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ROUT",
            operand_index = 4,
            operand_name = "feedback_2",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "ROUT",
            operand_index = 5,
            operand_name = "fault_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RTO",
            operand_index = 0,
            operand_name = "timer",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RTO",
            operand_index = 1,
            operand_name = "preset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RTO",
            operand_index = 2,
            operand_name = "accum",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RTOS",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "RTOS",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SBR",
            operand_index = 0,
            operand_name = "inputs",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SFP",
            operand_index = 0,
            operand_name = "SFC_routine_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SFP",
            operand_index = 1,
            operand_name = "target_state",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SFR",
            operand_index = 0,
            operand_name = "SFC_routine_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SFR",
            operand_index = 1,
            operand_name = "step_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SIN",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SIN",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SIZE",
            operand_index = 0,
            operand_name = "souce",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SIZE",
            operand_index = 1,
            operand_name = "dimension_to_vary",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SIZE",
            operand_index = 2,
            operand_name = "size",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SMAT",
            operand_index = 0,
            operand_name = "smat_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SMAT",
            operand_index = 1,
            operand_name = "restart_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SMAT",
            operand_index = 2,
            operand_name = "short_circuit_detect_delay_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SMAT",
            operand_index = 3,
            operand_name = "channel_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SMAT",
            operand_index = 4,
            operand_name = "channel_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SMAT",
            operand_index = 5,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SMAT",
            operand_index = 6,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQI",
            operand_index = 0,
            operand_name = "array",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQI",
            operand_index = 1,
            operand_name = "mask",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQI",
            operand_index = 2,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQI",
            operand_index = 3,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SQI",
            operand_index = 4,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQI",
            operand_index = 5,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQL",
            operand_index = 0,
            operand_name = "array",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQL",
            operand_index = 1,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQL",
            operand_index = 2,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SQL",
            operand_index = 3,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQL",
            operand_index = 4,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQO",
            operand_index = 0,
            operand_name = "array",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQO",
            operand_index = 1,
            operand_name = "mask",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQO",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SQO",
            operand_index = 3,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SQO",
            operand_index = 4,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQO",
            operand_index = 5,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQR",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQR",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SQRT",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SQRT",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SRT",
            operand_index = 0,
            operand_name = "array",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SRT",
            operand_index = 1,
            operand_name = "dim_to_vary",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SRT",
            operand_index = 2,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SRT",
            operand_index = 3,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SRT",
            operand_index = 4,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SSV",
            operand_index = 0,
            operand_name = "class_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SSV",
            operand_index = 1,
            operand_name = "instance_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SSV",
            operand_index = 2,
            operand_name = "attribute_name",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SSV",
            operand_index = 3,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "STD",
            operand_index = 0,
            operand_name = "array",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "STD",
            operand_index = 1,
            operand_name = "dim_to_vary",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "STD",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "STD",
            operand_index = 3,
            operand_name = "control",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "STD",
            operand_index = 4,
            operand_name = "length",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "STD",
            operand_index = 5,
            operand_name = "position",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "STOD",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "STOD",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "STOR",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "STOR",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SUB",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SUB",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SUB",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "SWPB",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SWPB",
            operand_index = 1,
            operand_name = "order_mode",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "SWPB",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "TAN",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TAN",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "THRS",
            operand_index = 0,
            operand_name = "thrs_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "THRS",
            operand_index = 1,
            operand_name = "active_pin_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRS",
            operand_index = 2,
            operand_name = "active_pin",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRS",
            operand_index = 3,
            operand_name = "right_button_normally_open",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRS",
            operand_index = 4,
            operand_name = "right_button_normally_closed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRS",
            operand_index = 5,
            operand_name = "left_button_normally_open",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRS",
            operand_index = 6,
            operand_name = "left_button_normally_closed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRS",
            operand_index = 7,
            operand_name = "fault_reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 0,
            operand_name = "thrse_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 1,
            operand_name = "discprepancy_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 2,
            operand_name = "enable",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 3,
            operand_name = "disconnected",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 4,
            operand_name = "right_button_normally_open",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 5,
            operand_name = "right_button_normally_closed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 6,
            operand_name = "left_button_normally_open",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 7,
            operand_name = "left_button_normally_closed",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 8,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "THRSE",
            operand_index = 9,
            operand_name = "resest",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TOD",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TOD",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "TOF",
            operand_index = 0,
            operand_name = "timer",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TOF",
            operand_index = 1,
            operand_name = "preset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TOF",
            operand_index = 2,
            operand_name = "accum",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TON",
            operand_index = 0,
            operand_name = "timer",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TON",
            operand_index = 1,
            operand_name = "preset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TON",
            operand_index = 2,
            operand_name = "accum",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TRN",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TRN",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "TRUNC",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TRUNC",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 0,
            operand_name = "tsam_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 1,
            operand_name = "restart_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 2,
            operand_name = "S1_S2_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 3,
            operand_name = "S2_LC_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 4,
            operand_name = "maximum_mute_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 5,
            operand_name = "maximum_override_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 6,
            operand_name = "light_curtain",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 7,
            operand_name = "sensor_1",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 8,
            operand_name = "sensor_2",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 9,
            operand_name = "enable_mute",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 10,
            operand_name = "override",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 11,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 12,
            operand_name = "muting_lamp_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSAM",
            operand_index = 13,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 0,
            operand_name = "tssm_tag",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 1,
            operand_name = "restart_type",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 2,
            operand_name = "S1_S2_discrepancy_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 3,
            operand_name = "S1_S2_LC_minimum_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 4,
            operand_name = "S1_S2_LC_maximum_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 5,
            operand_name = "maximum_mute_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 6,
            operand_name = "maximum_override_time",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 7,
            operand_name = "light_curtain",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 8,
            operand_name = "sensor_1",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 9,
            operand_name = "sensor_2",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 10,
            operand_name = "enable_mute",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 11,
            operand_name = "override",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 12,
            operand_name = "input_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 13,
            operand_name = "muting_lamp_status",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "TSSM",
            operand_index = 14,
            operand_name = "reset",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "UPPER",
            operand_index = 0,
            operand_name = "source",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "UPPER",
            operand_index = 1,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "XIC",
            operand_index = 0,
            operand_name = "data_bit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "XIO",
            operand_index = 0,
            operand_name = "data_bit",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "XOR",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "XOR",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "XOR",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "XPY",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "XPY",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "XPY",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
        insert.Row(new
        {
            instruction_key = "EXPT",
            operand_index = 0,
            operand_name = "source_A",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EXPT",
            operand_index = 1,
            operand_name = "source_B",
            is_destructive = false
        });
        insert.Row(new
        {
            instruction_key = "EXPT",
            operand_index = 2,
            operand_name = "destination",
            is_destructive = true
        });
    }
}