using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

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
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202603061300CreateArgumentTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("argument")
            .WithPrimaryGuid("argument_id")
            .WithSnapshotRelation()
            .WithParentRelation("instruction_id", "instruction")
            .WithColumn("argument_index").AsByte().NotNullable()
            .WithColumn("argument_type").AsString(32).NotNullable()
            .WithColumn("argument_text").AsString(255).NotNullable();

        Create.Index().OnTable("argument")
            .OnColumn("instruction_id").Ascending()
            .OnColumn("argument_index").Ascending();

        Create.Index().OnTable("argument")
            .OnColumn("argument_text").Ascending();
    }
}