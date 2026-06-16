using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202605271005, "Create tags_at_version function")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202605271005CreateTagsAtVersion : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.tags_at_version (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT t.* 
                        FROM dbo.tag t
                        JOIN dbo.target_version_map tvm ON t.tag_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'tag'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.tags_at_version;");
    }
}