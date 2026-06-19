CREATE OR ALTER FUNCTION [logix].[tasks_at_version] (@VersionId INT)
RETURNS TABLE AS RETURN (
    SELECT t.* 
    FROM [logix].[task] t
    JOIN [logix].[target_version_map] tvm ON t.task_id = tvm.record_id
    JOIN [logix].[target_component] tc ON tvm.component_id = tc.component_id
    WHERE tvm.version_id = @VersionId 
      AND tc.component_name = 'task'
);
