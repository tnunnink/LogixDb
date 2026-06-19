CREATE OR ALTER FUNCTION [logix].[controllers_at_version] (@VersionId INT)
RETURNS TABLE AS RETURN (
    SELECT c.* 
    FROM [logix].[controller] c
    JOIN [logix].[target_version_map] tvm ON c.controller_id = tvm.record_id
    JOIN [logix].[target_component] tc ON tvm.component_id = tc.component_id
    WHERE tvm.version_id = @VersionId 
      AND tc.component_name = 'controller'
);
