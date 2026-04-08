using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260306;

/// <summary>
/// Represents a database migration that creates the instruction table for storing instruction data
/// related to rungs in the Logix system.
/// </summary>
/// <remarks>
/// This migration creates the "instruction" table with columns for instruction identification,
/// text representation, and metadata about the instruction's characteristics (destructive and native flags).
/// The table includes a primary key and a foreign key relationship to the snapshot table.
/// </remarks>
[UsedImplicitly]
[Migration(202603061200, "Creates instruction table with corresponding indexes and keys")]
[Tags(TagBehavior.RequireAny, MigrationTag.Rung)]
public class M202603061200CreateInstructionTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("instruction")
            .WithPrimaryGuid("instruction_id")
            .WithRequiredRelation("rung_id", "rung")
            .WithColumn("instruction_index").AsInt16().NotNullable()
            .WithColumn("instruction_text").AsString(int.MaxValue).NotNullable()
            .WithColumn("instruction_key").AsString(128).NotNullable()
            .WithColumn("is_conditional").AsBoolean().NotNullable()
            .WithColumn("is_native").AsBoolean().NotNullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("instruction")
            .OnColumn("rung_id").Ascending()
            .OnColumn("instruction_index").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("instruction")
            .OnColumn("record_hash").Ascending()
            .OnColumn("rung_id").Ascending();
    }
}