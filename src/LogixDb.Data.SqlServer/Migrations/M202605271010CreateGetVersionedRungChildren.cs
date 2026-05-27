using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271010, "Create versioned helper functions for rung instructions, arguments and references")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202605271010CreateGetVersionedRungChildren : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedInstructions (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT ri.* 
                        FROM dbo.rung_instruction ri
                        JOIN dbo.rung r ON ri.rung_id = r.rung_id
                        JOIN dbo.routine ro ON r.routine_id = ro.routine_id
                        JOIN dbo.target_version_map tvm ON ro.routine_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'routine')
                    );
                    """);

        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedArguments (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT ra.* 
                        FROM dbo.rung_argument ra
                        JOIN dbo.rung r ON ra.rung_id = r.rung_id
                        JOIN dbo.routine ro ON r.routine_id = ro.routine_id
                        JOIN dbo.target_version_map tvm ON ro.routine_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'routine')
                    );
                    """);

        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedReferences (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT rr.* 
                        FROM dbo.rung_reference rr
                        JOIN dbo.rung r ON rr.rung_id = r.rung_id
                        JOIN dbo.routine ro ON r.routine_id = ro.routine_id
                        JOIN dbo.target_version_map tvm ON ro.routine_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'routine')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedInstructions;");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedArguments;");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedReferences;");
    }
}
