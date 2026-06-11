using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271012, "Create versioned helper functions for AOI parameters and rungs")]
[Tags(TagBehavior.RequireAny, MigrationTag.Aoi)]
public class M202605271012CreateAoiChildrenFor : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.aoi_parameters_for (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT ap.* 
                        FROM dbo.aoi_parameter ap
                        JOIN dbo.aoi a ON ap.aoi_id = a.aoi_id
                        JOIN dbo.target_version_map tvm ON a.aoi_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'aoi'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.aoi_parameters_for;");
    }
}