using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260322;

[UsedImplicitly]
[Migration(202603220500, "Creates tag_producer table with corresponding indexes")]
[Tags(TagBehavior.RequireAny, MigrationTag.Component, MigrationTag.Tag)]
public class M021CreateTagProducerTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_producer")
            .WithPrimaryGuid("producer_id")
            .WithNumericCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("tag_id").AsGuid().NotNullable()
            .WithColumn("produce_count").AsInt32().NotNullable()
            .WithColumn("programatically_send_event_trigger").AsBoolean().NotNullable()
            .WithColumn("unicast_permitted").AsBoolean().NotNullable()
            .WithColumn("maximum_rpi").AsDouble().NotNullable()
            .WithColumn("minimum_rpi").AsDouble().NotNullable()
            .WithColumn("default_rpi").AsDouble().NotNullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("tag_producer")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("tag_id").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_producer")
            .OnColumn("record_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}