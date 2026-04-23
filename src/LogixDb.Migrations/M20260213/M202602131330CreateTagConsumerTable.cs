using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260213;

[UsedImplicitly]
[Migration(202602131330, "Creates tag_consumer table with corresponding indexes")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202602131330CreateTagConsumerTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_consumer")
            .WithPrimaryKey("consumer_id")
            .WithRelation("instance_id", "target_instance").OnDelete(Rule.Cascade).NotNullable()
            .WithRelation("tag_id", "tag").NotNullable()
            .WithColumn("producer").AsString().NotNullable()
            .WithColumn("remote_tag").AsString().NotNullable()
            .WithColumn("remote_instance").AsInt32().NotNullable()
            .WithColumn("rpi").AsDouble().NotNullable()
            .WithColumn("unicast").AsBoolean().NotNullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("tag_consumer")
            .OnColumn("tag_id").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_consumer")
            .OnColumn("tag_id").Ascending()
            .OnColumn("record_hash").Ascending();
    }
}