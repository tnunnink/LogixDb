using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271012, "Create versioned helper functions for AOI parameters and rungs")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202605271012CreateGetVersionedAoiChildren : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedAoiParameters (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT ap.* 
                        FROM dbo.aoi_parameter ap
                        JOIN dbo.aoi a ON ap.aoi_id = a.aoi_id
                        JOIN dbo.target_version_map tvm ON a.aoi_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'aoi')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedAoiParameters;");
    }
}