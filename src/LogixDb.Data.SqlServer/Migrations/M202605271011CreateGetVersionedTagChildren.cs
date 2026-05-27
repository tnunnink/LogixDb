using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271011, "Create versioned helper functions for tag members, values and comments")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
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
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'tag'
                    );
                    """);

        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedTagComments (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT c.* 
                        FROM dbo.tag_comment c
                        JOIN dbo.tag t ON c.tag_id = t.tag_id
                        JOIN dbo.target_version_map tvm ON t.tag_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'tag'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedTagMembers;");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedTagComments;");
    }
}