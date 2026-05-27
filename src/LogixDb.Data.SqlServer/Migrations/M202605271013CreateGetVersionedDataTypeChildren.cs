using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271013, "Create versioned helper function for data type members")]
[Tags(TagBehavior.RequireAny, MigrationTag.DataType)]
public class M202605271013CreateGetVersionedDataTypeChildren : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedDataTypeMembers (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT dtm.* 
                        FROM dbo.data_type_member dtm
                        JOIN dbo.data_type dt ON dtm.type_id = dt.type_id
                        JOIN dbo.target_version_map tvm ON dt.type_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id
                        WHERE tvm.version_id = @VersionId 
                          AND tc.component_name = 'data_type'
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedDataTypeMembers;");
    }
}
