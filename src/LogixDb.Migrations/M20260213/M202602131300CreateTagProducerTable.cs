using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260213;

[UsedImplicitly]
[Migration(202602131300, "Creates tag_producer table with corresponding indexes")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202602131300CreateTagProducerTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_producer")
            .WithPrimaryKey<long>("producer_id")
            .WithRelation<long>("tag_id", "tag").NotNullable()
            .WithColumn("produce_count").AsInt32().NotNullable()
            .WithColumn("send_event_trigger").AsBoolean().NotNullable()
            .WithColumn("unicast_permitted").AsBoolean().NotNullable()
            .WithColumn("maximum_rpi").AsDouble().NotNullable()
            .WithColumn("minimum_rpi").AsDouble().NotNullable()
            .WithColumn("default_rpi").AsDouble().NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("tag_producer")
            .OnColumn("tag_id").Ascending()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();
    }
}