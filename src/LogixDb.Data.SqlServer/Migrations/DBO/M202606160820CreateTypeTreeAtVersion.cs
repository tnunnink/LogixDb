using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202606160820, "Create type_tree_at_version function")]
[Tags(TagBehavior.RequireAny, MigrationTag.DataType)]
public class M202606160820CreateTypeTreeAtVersion : Migration
{
    public override void Up()
    {
        Execute.Sql("""
            CREATE OR ALTER FUNCTION dbo.type_tree_at_version(
                @VersionId INT
            )
                RETURNS TABLE AS RETURN(WITH type_tree AS (
                    -- Anchor: get all root data types for the specified version.
                    SELECT
                        dt.type_id                  [root_id],
                        CAST(NULL AS BIGINT)        [parent_id],
                        dt.type_id                  [resolved_id],
                        dt.type_name                [root_type],
                        CAST(NULL AS NVARCHAR(256)) [parent_type],
                        CAST('' AS NVARCHAR(256))   [member_path],
                        CAST(NULL AS NVARCHAR(256)) [member_name],
                        dt.type_name                [data_type],
                        dt.type_description         [root_description],
                        CAST(NULL AS NVARCHAR(MAX)) [member_description]
                    FROM dbo.data_types_at_version(@VersionId) dt

                    UNION ALL

                    -- Recursively add child members and resolve their member type for the current version.
                    SELECT
                        tt.root_id,
                        dt.type_id,
                        ct.type_id,
                        tt.root_type,
                        dt.type_name,
                        CAST(dbo.tag_concat(tt.member_path, ctm.member_name) AS NVARCHAR(256)),
                        ctm.member_name,
                        ctm.data_type,
                        tt.root_description,
                        COALESCE(ctm.member_description, ct.type_description, tt.member_description)
                    FROM type_tree tt
                         JOIN data_type dt ON dt.type_id = tt.resolved_id
                         JOIN data_type_member ctm ON ctm.type_id = dt.type_id
                         OUTER APPLY (
                            SELECT *
                            FROM dbo.data_types_at_version(@VersionId)
                            WHERE type_name = ctm.data_type
                            ) ct
                    WHERE ctm.is_hidden = 0
                    )

                    SELECT *
                    FROM type_tree
                )
            """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.type_tree_at_version;");
    }
}
