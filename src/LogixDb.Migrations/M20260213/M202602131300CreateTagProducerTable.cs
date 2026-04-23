using System.Data;
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
            .WithPrimaryKey("producer_id")
            .WithRelation("instance_id", "target_instance").OnDelete(Rule.Cascade).NotNullable()
            .WithRelation("tag_id", "tag").NotNullable()
            .WithColumn("produce_count").AsInt32().NotNullable()
            .WithColumn("send_event_trigger").AsBoolean().NotNullable()
            .WithColumn("unicast_permitted").AsBoolean().NotNullable()
            .WithColumn("maximum_rpi").AsDouble().NotNullable()
            .WithColumn("minimum_rpi").AsDouble().NotNullable()
            .WithColumn("default_rpi").AsDouble().NotNullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("tag_producer")
            .OnColumn("tag_id").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_producer")
            .OnColumn("tag_id").Ascending()
            .OnColumn("record_hash").Ascending();
    }
}