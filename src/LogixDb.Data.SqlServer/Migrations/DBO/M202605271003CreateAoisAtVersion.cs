using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202605271003, "Create aois_at_version function")]
[Tags(TagBehavior.RequireAny, MigrationTag.Aoi)]
public class M202605271003CreateAoisAtVersion : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.aois_at_version (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT a.* 
                        FROM dbo.aoi a
                        JOIN dbo.target_version_map tvm ON a.aoi_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'aoi'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.aois_at_version;");
    }
}