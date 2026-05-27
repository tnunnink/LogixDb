using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271005, "Create versioned helper function for tag")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202605271005CreateGetVersionedTags : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedTags (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT t.* 
                        FROM dbo.tag t
                        JOIN dbo.target_version_map tvm ON t.tag_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'tag')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedTags;");
    }
}