using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260206;

/// <summary>
/// Creates the snapshot_registry table to track when snapshots were restored and by whom.
/// This table maintains a historical record of snapshot restoration events.
/// </summary>
[UsedImplicitly]
[Migration(202602061040, "Create snapshot_instance table to link snapshot to component tables")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202602061040CreateSnapshotInstanceTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("snapshot_instance")
            .WithColumn("snapshot_id").AsInt32().PrimaryKey().ForeignKey("snapshot", "snapshot_id")
            .WithColumn("restored_on").AsDateTime().NotNullable()
            .WithColumn("restored_by").AsString(64).NotNullable();
    }
}