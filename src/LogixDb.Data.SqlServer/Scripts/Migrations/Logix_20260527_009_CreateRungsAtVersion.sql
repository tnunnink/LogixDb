CREATE OR ALTER FUNCTION [logix].[rungs_at_version] (@VersionId INT)
RETURNS TABLE AS RETURN (
    SELECT r.* 
    FROM [logix].[rung] r
    JOIN [logix].[target_version_map] tvm ON r.rung_id = tvm.record_id
    JOIN [logix].[target_component] tc ON tvm.component_id = tc.component_id
    WHERE tvm.version_id = @VersionId 
      AND tc.component_name = 'rung'
);
