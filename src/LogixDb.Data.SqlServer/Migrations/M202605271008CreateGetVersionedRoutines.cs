using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271008, "Create versioned helper function for routine")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
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
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'routine')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedRoutines;");
    }
}