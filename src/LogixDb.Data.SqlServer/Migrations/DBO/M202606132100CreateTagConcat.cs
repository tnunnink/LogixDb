using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202606132100, "Create tag_concat helper function for concatenating tags while accounting for array bracket")]
[Tags(TagBehavior.RequireAny, MigrationTag.Tag)]
public class M202606132100CreateTagConcat : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION [dbo].[tag_concat]
                    (
                    	@TagName NVARCHAR(256),
                    	@MemberPath NVARCHAR(256)
                    )
                    RETURNS NVARCHAR(256)
                    AS
                    BEGIN
                    	DECLARE @Separator NVARCHAR(1) = CASE
                            WHEN @MemberPath IS NULL OR @MemberPath = '' THEN ''
                            WHEN @TagName IS NULL OR @TagName = '' THEN ''
                            WHEN CHARINDEX('[', @MemberPath) = 1 THEN ''
                            ELSE '.'
                        END;
                        
                        DECLARE @TagPath NVARCHAR(256) = CONCAT(@TagName, @Separator, @MemberPath)
                        RETURN @TagPath
                    END;
                    """
        );
    }

    public override void Down()
    {
    Execute.Sql("DROP FUNCTION IF EXISTS [dbo].[tag_concat];");
    }
}