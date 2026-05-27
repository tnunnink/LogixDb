using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202605271006, "Create versioned helper function for task")]
[Tags(TagBehavior.RequireAny, MigrationTag.Required)]
public class M202605271006CreateGetVersionedTasks : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.GetVersionedTasks (@VersionId INT)
                    RETURNS TABLE AS RETURN (
                        SELECT t.* 
                        FROM dbo.task t
                        JOIN dbo.target_version_map tvm ON t.task_id = tvm.record_id
                        WHERE tvm.version_id = @VersionId 
                          AND tvm.component_id = (SELECT component_id FROM dbo.target_component WHERE component_name = 'task')
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.GetVersionedTasks;");
    }
}