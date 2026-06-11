using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271014, "Create versioned helper function for operands")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic, MigrationTag.Aoi)]
public class M202605271014CreateOperandsFor : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.operands_for (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT o.* 
                        FROM dbo.operand o
                        JOIN dbo.target_version_map tvm ON o.operand_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'operand'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.operands_for;");
    }
}