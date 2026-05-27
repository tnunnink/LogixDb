using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271007, "Create versioned helper function for program")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202605271007CreateGetVersionedPrograms : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedPrograms (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT p.* 
                        FROM dbo.program p
                        JOIN dbo.target_version_map tvm ON p.program_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'program')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedPrograms;");
    }
}