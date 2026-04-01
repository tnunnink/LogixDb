using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260322;

[UsedImplicitly]
[Migration(202603220530, "Creates tag_consumer table with corresponding indexes")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M022CreateTagConsumerTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("tag_consumer")
            .WithPrimaryGuid("consumer_id")
            .WithNumericCascadeForeignKey("snapshot_id", "snapshot")
            .WithColumn("tag_id").AsGuid().NotNullable()
            .WithColumn("producer").AsString().NotNullable()
            .WithColumn("remote_tag").AsString().NotNullable()
            .WithColumn("remote_instance").AsInt32().NotNullable()
            .WithColumn("rpi").AsDouble().NotNullable()
            .WithColumn("unicast").AsBoolean().NotNullable()
            .WithColumn("record_hash").AsString(32).NotNullable();

        Create.Index().OnTable("tag_consumer")
            .OnColumn("snapshot_id").Ascending()
            .OnColumn("tag_id").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("tag_consumer")
            .OnColumn("record_hash").Ascending()
            .OnColumn("snapshot_id").Ascending();
    }
}