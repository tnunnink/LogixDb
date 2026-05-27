using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271011, "Create versioned helper functions for tag members, values and comments")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202605271011CreateGetVersionedTagChildren : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedTagMembers (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT tm.* 
                        FROM dbo.tag_member tm
                        JOIN dbo.tag t ON tm.tag_id = t.tag_id
                        JOIN dbo.target_version_map tvm ON t.tag_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'tag')
                    );
                    """);

        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedTagValues (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT tv.* 
                        FROM dbo.tag_value tv
                        WHERE tv.version_id = @VersionId
                    );
                    """);

        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedTagComments (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT tc.* 
                        FROM dbo.tag_comment tc
                        JOIN dbo.tag t ON tc.tag_id = t.tag_id
                        JOIN dbo.target_version_map tvm ON t.tag_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'tag')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedTagMembers;");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedTagValues;");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedTagComments;");
    }
}
