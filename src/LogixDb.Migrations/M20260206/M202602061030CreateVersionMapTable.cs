using FluentMigrator;
using JetBrains.Annotations;
using LogixDb.Data;

namespace LogixDb.Migrations.M20260206;

[UsedImplicitly]
[Migration(202602061030, "Create version map table and associated indexes for version relationships")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202602061030CreateVersionMapTable : AutoReversingMigration
{
    public override void Up()
    {
        Create.Table("version_map")
            .WithRelation<Guid>("version_id", "version")
            .WithColumn("component_id").AsGuid().NotNullable()
            .WithColumn("component_type").AsString().NotNullable();

        Create.Index().OnTable("version_map")
            .OnColumn("version_id").Ascending()
            .OnColumn("component_id").Ascending()
            .OnColumn("component_type").Ascending()
            .WithOptions().Unique();

        Create.Index().OnTable("version_map")
            .OnColumn("component_id").Ascending()
            .OnColumn("component_type").Ascending();
    }
}