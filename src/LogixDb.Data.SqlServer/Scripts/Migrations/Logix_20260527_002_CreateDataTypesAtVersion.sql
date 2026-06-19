CREATE OR ALTER FUNCTION [logix].[data_types_at_version] (@VersionId INT)
RETURNS TABLE AS RETURN (
    SELECT dt.* 
    FROM [logix].[data_type] dt
    JOIN [logix].[target_version_map] tvm ON dt.type_id = tvm.record_id
    JOIN [logix].[target_component] tc ON tvm.component_id = tc.component_id
    WHERE tvm.version_id = @VersionId 
      AND tc.component_name = 'data_type'
);
