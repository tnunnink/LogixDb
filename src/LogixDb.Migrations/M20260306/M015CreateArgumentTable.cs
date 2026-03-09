using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260306;

/// <summary>
/// Represents a database migration that creates the argument table for storing instruction argument data
/// in the Logix system.
/// </summary>
/// <remarks>
/// This migration creates the "argument" table with columns for argument identification, type information,
/// text representation, tags, values, and metadata. Each argument is associated with a specific instruction
/// through the instruction_hash column and linked to a snapshot through a foreign key relationship.
/// The table includes indexes for efficient querying by snapshot_id and instruction_hash combinations,
/// as well as by record_hash and snapshot_id combinations.
/// </remarks>
[UsedImplicitly]
[Migration(202603061300, "Creates argument table with corresponding indexes and foreign key relationships")]
public class M015CreateArgumentTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("argument")
            .WithPrimaryId("argument_id")
            .WithCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("instruction_hash").AsString(32).NotNullable()
            .WithColumn("argument_index").AsByte().NotNullable()
            .WithColumn("argument_type").AsString(32).NotNullable()
            .WithColumn("argument_text").AsString(int.MaxValue).NotNullable()
            .WithColumn("argument_tags").AsString(int.MaxValue).Nullable()
            .WithColumn("argument_values").AsString(int.MaxValue).Nullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index()
            .OnTable("argument")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("instruction_hash").Ascending();
        
        Create.Index()
            .OnTable("argument")
            .OnColumn("record_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}