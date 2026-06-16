using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202605271001, "Create controllers_at_version function")]
[Tags(TagBehavior.RequireAny, MigrationTag.Controller)]
public class M202605271001CreateControllersAtVersion : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.controllers_at_version (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT c.* 
                        FROM dbo.controller c
                        JOIN dbo.target_version_map tvm ON c.controller_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'controller'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.controllers_at_version;");
    }
}