using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202605271002, "Create data_types_at_version function")]
[Tags(TagBehavior.RequireAny, MigrationTag.DataType)]
public class M202605271002CreateDataTypesAtVersion : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.data_types_at_version (@VersionId INT)
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
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.data_types_at_version;");
    }
}