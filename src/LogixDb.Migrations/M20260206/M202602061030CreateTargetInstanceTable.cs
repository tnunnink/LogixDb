using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260206;

[UsedImplicitly]
[Migration(202602061030, "Create target_instance table to link a target version to component tables")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202602061030CreateTargetInstanceTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("target_instance")
            .WithColumn("instance_id").AsInt32().PrimaryKey().Identity()
            .WithVersionRelation()
            .WithColumn("restored_on").AsDateTime().NotNullable()
            .WithColumn("restored_by").AsString(64).NotNullable();
    }
}