using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271002, "Create versioned helper function for data_type")]
[Tags(TagBehavior.RequireAny, MigrationTag.DataType)]
public class M202605271002CreateDataTypesFor : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.data_types_for (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT dt.* 
                        FROM dbo.data_type dt
                        JOIN dbo.target_version_map tvm ON dt.type_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'data_type'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.data_types_for;");
    }
}