using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260206;

[UsedImplicitly]
[Migration(202602060900, "Creates target table with unique target key index")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202602060900CreateTargetTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("target")
            .WithPrimaryGuid("target_id")
            .WithColumn("target_key").AsString(128).NotNullable()
            .WithColumn("created_on").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Index()
            .OnTable("target")
            .OnColumn("target_key").Ascending()
            .WithOptions().Unique();
    }
}