CREATE OR ALTER FUNCTION [logix].[modules_at_version] (@VersionId INT)
RETURNS TABLE AS RETURN (
    SELECT m.* 
    FROM [logix].[module] m
    JOIN [logix].[target_version_map] tvm ON m.module_id = tvm.record_id
    JOIN [logix].[target_component] tc ON tvm.component_id = tc.component_id
    WHERE tvm.version_id = @VersionId 
      AND tc.component_name = 'module'
);
