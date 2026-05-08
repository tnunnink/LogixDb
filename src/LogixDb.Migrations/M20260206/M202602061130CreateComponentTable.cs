using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260206;

[UsedImplicitly]
[Migration(202602061130, "Creates component_type table with primary key and component name column")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202602061130CreateComponentTypeTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("component")
            .WithPrimaryKey<byte>("component_id")
            .WithColumn("component_name").AsString(64).NotNullable();
    }
}