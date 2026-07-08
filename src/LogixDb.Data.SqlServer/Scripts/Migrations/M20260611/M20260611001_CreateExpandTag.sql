CREATE OR ALTER FUNCTION [logix].[expand_tag] (@MemberId BIGINT)
RETURNS TABLE AS RETURN (
    WITH TagTree AS (
        -- Anchor: Start with the specific member provided
        SELECT 
            tm.*, 
            0 as [member_distance]
        FROM [logix].[tag_member] tm
        WHERE tm.member_id = @MemberId

        UNION ALL

        -- Recursive Step: Join back to tag_member to find children
        -- We join on tag_id and member_path/parent_path to stay in scope
        SELECT 
            tm.*, 
            tt.member_distance + 1
        FROM TagTree tt
        JOIN [logix].[tag_member] tm ON tt.tag_id = tm.tag_id AND tt.member_path = tm.parent_path
        WHERE tm.member_name IS NOT NULL
    )
    SELECT * FROM TagTree
);
