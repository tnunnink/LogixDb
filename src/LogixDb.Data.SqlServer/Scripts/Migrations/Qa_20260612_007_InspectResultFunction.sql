CREATE FUNCTION [qa].[inspect_result]
(
    @result_id BIGINT
)
    RETURNS TABLE
        AS
        RETURN
        (
        WITH RecursiveJSON AS (
            SELECT
                TRY_CAST('' AS NVARCHAR(4000)) COLLATE DATABASE_DEFAULT AS parent_node,
                j.[key] AS node_key,
                j.[value] AS node_value,
                j.[type] AS node_type,
                0 AS node_level
            FROM OPENJSON((SELECT result_details FROM [qa].validation_result WHERE result_id = @result_id)) j

            UNION ALL

            SELECT
                TRY_CAST(p.node_key AS NVARCHAR(4000)) COLLATE DATABASE_DEFAULT AS parent_node,
                c.[key] AS node_key,
                c.[value] AS node_value,
                c.[type] AS node_type,
                p.node_level + 1 AS node_level
            FROM RecursiveJSON p
                 CROSS APPLY OPENJSON(p.node_value) c
            WHERE p.node_type = 5
        )

        SELECT *
        FROM RecursiveJSON
        WHERE node_type <> 5
        )
GO
