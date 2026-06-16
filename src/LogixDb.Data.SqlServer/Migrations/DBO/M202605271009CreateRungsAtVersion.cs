using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202605271009, "Create rungs_at_version function")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202605271009CreateRungsAtVersion : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.rungs_at_version (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT r.* 
                        FROM dbo.rung r
                        JOIN dbo.target_version_map tvm ON r.rung_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'rung'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.rungs_at_version;");
    }
}