CREATE OR ALTER FUNCTION [logix].[aois_at_version] (@VersionId INT)
RETURNS TABLE AS RETURN (
    SELECT a.* 
    FROM [logix].[aoi] a
    JOIN [logix].[target_version_map] tvm ON a.aoi_id = tvm.record_id
    JOIN [logix].[target_component] tc ON tvm.component_id = tc.component_id
    WHERE tvm.version_id = @VersionId 
      AND tc.component_name = 'aoi'
);
