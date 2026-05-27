using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271008, "Create versioned helper function for routine")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202605271008CreateGetVersionedRoutines : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedRoutines (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT r.* 
                        FROM dbo.routine r
                        JOIN dbo.target_version_map tvm ON r.routine_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'routine'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedRoutines;");
    }
}