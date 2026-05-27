using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271014, "Create versioned helper function for operands")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202605271014CreateGetVersionedOperands : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedOperands (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT o.* 
                        FROM dbo.operand o
                        JOIN dbo.target_version_map tvm ON o.operand_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'operand')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedOperands;");
    }
}