using System.Data;
using FluentMigrator;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260611;


[Migration(202606112100, "Creates module connection table")]
[Tags(TagBehavior.RequireAny, MigrationTag.Module)]
public class M202606112100CreateModuleConnectionTable : AutoReversingMigration 
{
    public override void Up()
    {
        Create.Table("module_connection")
            .WithPrimaryKey<long>("connection_id")
            .WithRelation<long>("module_id", "module").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("connection_name").AsString(256).NotNullable()
            .WithColumn("rpi").AsInt32().NotNullable()
            .WithColumn("connection_type").AsString(64).Nullable()
            .WithColumn("connection_priority").AsString(64).Nullable()
            .WithColumn("transmission_type").AsString(64).Nullable()
            .WithColumn("production_trigger").AsString(64).Nullable()
            .WithColumn("output_redundant_owner").AsBoolean().Nullable()
            .WithColumn("unicast").AsBoolean().Nullable()
            .WithColumn("programatically_send_event_trigger").AsBoolean().Nullable()
            .WithColumn("event_id").AsInt32().Nullable()
            .WithColumn("input_tag").AsString(256).Nullable()
            .WithColumn("input_size").AsInt32().Nullable()
            .WithColumn("input_suffix").AsString(64).Nullable()
            .WithColumn("output_tag").AsString(256).Nullable()
            .WithColumn("output_size").AsInt32().Nullable()
            .WithColumn("output_suffix").AsString(64).Nullable()
            .WithColumn("connection_path").AsString(512).Nullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("module_connection")
            .OnColumn("module_id").Ascending()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();
    }
}