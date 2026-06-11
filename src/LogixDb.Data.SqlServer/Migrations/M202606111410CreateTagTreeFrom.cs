using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations;

[UsedImplicitly]
[Migration(202606111410, "Create tag_tree_from recursive function for hierarchical tag exploration")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202606111410CreateTagTreeFrom : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.tag_tree_from (@RootMemberId BIGINT)
                    RETURNS TABLE AS RETURN (
                        WITH TagTree AS (
                            -- Anchor: Start with the specific member provided
                            SELECT 
                                tm.*, 
                                0 as [member_distance]
                            FROM dbo.tag_member tm
                            WHERE tm.member_id = @RootMemberId

                            UNION ALL

                            -- Recursive Step: Join back to tag_member to find children
                            -- We join on tag_id and tag_name/parent_name to stay in scope
                            SELECT 
                                tm.*, 
                                tt.member_distance + 1
                            FROM TagTree tt
                            JOIN dbo.tag_member tm ON tt.tag_id = tm.tag_id 
                                                   AND tt.tag_name = tm.parent_name
                        )
                        SELECT * FROM TagTree
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.tag_tree_from;");
    }
}
