using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271010, "Create versioned helper functions for rung instructions, arguments and references")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202605271010CreateRungChildrenFor : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.instructions_for (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT ri.* 
                        FROM dbo.rung_instruction ri
                        JOIN dbo.rung r ON ri.rung_id = r.rung_id
                        JOIN dbo.routine ro ON r.routine_id = ro.routine_id
                        JOIN dbo.target_version_map tvm ON ro.routine_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'routine'
                    );
                    """);

        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.arguments_for (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT ra.* 
                        FROM dbo.rung_argument ra
                        JOIN dbo.rung r ON ra.rung_id = r.rung_id
                        JOIN dbo.routine ro ON r.routine_id = ro.routine_id
                        JOIN dbo.target_version_map tvm ON ro.routine_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'routine'
                    );
                    """);

        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.references_for (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT rr.* 
                        FROM dbo.rung_reference rr
                        JOIN dbo.rung r ON rr.rung_id = r.rung_id
                        JOIN dbo.routine ro ON r.routine_id = ro.routine_id
                        JOIN dbo.target_version_map tvm ON ro.routine_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'routine'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.instructions_for;");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.arguments_for;");
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.references_for;");
    }
}
