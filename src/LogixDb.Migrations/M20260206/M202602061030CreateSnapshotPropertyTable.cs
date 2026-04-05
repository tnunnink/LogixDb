using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260206;

/// <summary>
/// Creates the snapshot_info table for storing key-value metadata associated with snapshots.
/// </summary>
[UsedImplicitly]
[Migration(202602061030, "Create snapshot_property table for snapshot metadata")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202602061030CreateSnapshotPropertyTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("snapshot_property")
            .WithPrimaryGuid("property_id")
            .WithSnapshotRelation()
            .WithColumn("property_name").AsString().NotNullable()
            .WithColumn("property_value").AsString().Nullable();

        Create.Index()
            .OnTable("snapshot_property")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("property_name").Ascending()
            .WithOptions().Unique();
    }
}