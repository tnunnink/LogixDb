using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271001, "Create versioned helper function for controller")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202605271001CreateGetVersionedControllers : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedControllers (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT c.* 
                        FROM dbo.controller c
                        JOIN dbo.target_version_map tvm ON c.controller_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'controller')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedControllers;");
    }
}