using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Migrations.M20260206;

[UsedImplicitly]
[Migration(202602061010, "Creates target table with unique target key index")]
public class M001CreateTargetTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("target")
            .WithPrimaryId("target_id")
            .WithColumn("target_key").AsString(128).NotNullable()
            .WithColumn("created_on").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.Index()
            .OnTable("target")
            .OnColumn("target_key").Ascending()
            .WithOptions().Unique();
    }
}