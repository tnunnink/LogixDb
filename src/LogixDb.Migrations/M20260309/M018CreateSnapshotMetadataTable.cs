using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260309;

/// <summary>
/// Creates the snapshot_info table for storing key-value metadata associated with snapshots.
/// </summary>
[UsedImplicitly]
//[Migration(202603092030, "Create snapshot_info table for snapshot metadata")]
public class M018CreateSnapshotMetadataTable : AutoReversingMigration
{
    /// <summary>
    /// Creates the snapshot_info table with columns for storing metadata key-value pairs linked to snapshots.
    /// </summary>
    public override void Up()
    {
        Create.Table("snapshot_info")
            .WithPrimaryId("info_id")
            .WithCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("key").AsString(128).NotNullable()
            .WithColumn("value").AsString().Nullable();
    }
}