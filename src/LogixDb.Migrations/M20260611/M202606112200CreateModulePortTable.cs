using System.Data;
using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260611;

[UsedImplicitly]
[Migration(202606112200, "Creates module port table")]
[Tags(TagBehavior.RequireAny, MigrationTag.Module)]
public class M202606112200CreateModulePortTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("module_port")
            .WithPrimaryKey<long>("port_id")
            .WithRelation<long>("module_id", "module").OnDelete(Rule.Cascade).NotNullable()
            .WithColumn("port_number").AsInt16().NotNullable()
            .WithColumn("port_type").AsString(64).Nullable()
            .WithColumn("address").AsString(256).Nullable()
            .WithColumn("upstream").AsBoolean().Nullable()
            .WithColumn("bus_size").AsByte().Nullable()
            .WithColumn("record_hash").AsString(64).NotNullable();

        Create.Index().OnTable("module_port")
            .OnColumn("module_id").Ascending()
            .OnColumn("record_hash").Ascending()
            .WithOptions().Unique();
    }
}