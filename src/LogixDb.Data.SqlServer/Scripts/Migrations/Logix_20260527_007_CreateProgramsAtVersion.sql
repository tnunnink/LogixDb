CREATE OR ALTER FUNCTION [logix].[programs_at_version] (@VersionId INT)
RETURNS TABLE AS RETURN (
    SELECT p.* 
    FROM [logix].[program] p
    JOIN [logix].[target_version_map] tvm ON p.program_id = tvm.record_id
    JOIN [logix].[target_component] tc ON tvm.component_id = tc.component_id
    WHERE tvm.version_id = @VersionId 
      AND tc.component_name = 'program'
);
