using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260308;

/// <summary>
/// Represents a database migration that creates the operand table for storing instruction operand metadata
/// in the Logix system.
/// </summary>
/// <remarks>
/// This migration creates the "operand" table with columns for operand identification, type information,
/// format specifications, and descriptions. Each operand is associated with a specific instruction through
/// the instruction_key column and ordered by operand_index. The table includes metadata about whether
/// the operand is destructive (modifies the value). A unique composite index is created on instruction_key
/// and operand_index to ensure each instruction's operands are uniquely identified by their position.
/// </remarks>
[UsedImplicitly]
[Migration(202603082100, "Creates operand table with unique composite index on instruction_key and operand_index")]
public class M016CreateOperandTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("operand")
            .WithPrimaryId("operand_id")
            .WithOptionalCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("instruction_key").AsString(128).NotNullable()
            .WithColumn("operand_index").AsByte().NotNullable()
            .WithColumn("operand_name").AsString(128).NotNullable()
            .WithColumn("operand_type").AsString(128).Nullable()
            .WithColumn("operand_format").AsString(32).Nullable()
            .WithColumn("operand_description").AsString(2000).Nullable()
            .WithColumn("is_destructive").AsBoolean().NotNullable();

        Create.Index()
            .OnTable("operand")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("instruction_key").Ascending()
            .OnColumn("operand_index").Ascending()
            .WithOptions().Unique();
    }
}