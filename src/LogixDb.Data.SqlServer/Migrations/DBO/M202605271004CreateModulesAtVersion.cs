using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202605271004, "Create modules_at_version function")]
[Tags(TagBehavior.RequireAny, MigrationTag.Module)]
public class M202605271004CreateModulesAtVersion : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.modules_at_version (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT m.* 
                        FROM dbo.module m
                        JOIN dbo.target_version_map tvm ON m.module_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'module'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.modules_at_version;");
    }
}