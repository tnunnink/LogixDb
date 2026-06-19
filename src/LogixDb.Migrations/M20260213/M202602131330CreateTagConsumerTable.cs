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
        Create.Table("tag_consumer").InLogixSchema()
            .WithRelation<long>("tag_id", "tag").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("producer").AsString().NotNullable()
            .WithColumn("remote_tag").AsString().NotNullable()
            .WithColumn("remote_instance").AsInt32().NotNullable()
            .WithColumn("rpi").AsDouble().NotNullable()
            .WithColumn("unicast").AsBoolean().NotNullable()
            .WithColumn("record_hash").AsString(64).NotNullable();
    }
}