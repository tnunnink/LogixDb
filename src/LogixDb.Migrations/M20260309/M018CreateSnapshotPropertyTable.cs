using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260309;

/// <summary>
/// Creates the snapshot_info table for storing key-value metadata associated with snapshots.
/// </summary>
[UsedImplicitly]
[Migration(202603092030, "Create snapshot_info table for snapshot metadata")]
public class M018CreateSnapshotPropertyTable : AutoReversingMigration
{
    /// <summary>
    /// Creates the snapshot_info table with columns for storing metadata key-value pairs linked to snapshots.
    /// </summary>
    public override void Up()
    {
        Create.Table("snapshot_property")
            .WithPrimaryId("property_id")
            .WithCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("property_name").AsString().NotNullable()
            .WithColumn("property_value").AsString().Nullable();
        
        // Ensure we can't have duplicate keys for the same snapshot and speed up lookups
        Create.Index()
            .OnTable("snapshot_property")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("property_name").Ascending()
            .WithOptions().Unique();
    }
}