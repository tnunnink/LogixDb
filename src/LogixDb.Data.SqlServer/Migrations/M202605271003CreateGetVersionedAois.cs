using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271003, "Create versioned helper function for aoi")]
[Tags(TagBehavior.RequireAny, MigrationTag.Aoi)]
public class M202605271003CreateGetVersionedAois : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedAois (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT a.* 
                        FROM dbo.aoi a
                        JOIN dbo.target_version_map tvm ON a.aoi_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'aoi')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedAois;");
    }
}