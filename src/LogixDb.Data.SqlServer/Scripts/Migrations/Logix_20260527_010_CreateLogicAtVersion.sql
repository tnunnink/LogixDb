CREATE OR ALTER FUNCTION [logix].[logic_at_version] (@VersionId INT)
RETURNS TABLE AS RETURN (
    SELECT 
        r.rung_id,
        r.container_name,
        r.routine_name,
        r.rung_number,
        ri.instruction_index,
        ri.instruction_key,
        ri.instruction_text,
        ra.argument_index,
        ra.argument_type,
        ra.argument_text,
        rr.reference_name,
        o.operand_name,
        o.operand_type,
        ri.is_conditional,
        ri.is_native,
        o.is_destructive
    FROM [logix].[rung] r
    JOIN [logix].[target_version_map] tvm ON r.rung_id = tvm.record_id
    JOIN [logix].[target_component] tc ON tvm.component_id = tc.component_id AND tc.component_name = 'rung'
    JOIN [logix].[rung_instruction] ri ON r.rung_id = ri.rung_id
    LEFT JOIN [logix].[rung_argument] ra ON ri.rung_id = ra.rung_id 
        AND ri.instruction_index = ra.instruction_index
    LEFT JOIN [logix].[rung_reference] rr ON ra.rung_id = rr.rung_id 
        AND ra.instruction_index = rr.instruction_index 
        AND ra.argument_index = rr.argument_index
    LEFT JOIN [logix].[operand] o ON ri.instruction_key = o.instruction_key 
        AND ra.argument_index = o.operand_index
        AND (
            o.is_native = 1
            OR EXISTS (
                SELECT 1 FROM [logix].[target_version_map] tvmo 
                JOIN [logix].[target_component] tco ON tvmo.component_id = tco.component_id
                WHERE tvmo.record_id = o.operand_id 
                  AND tvmo.version_id = @VersionId
                  AND tco.component_name = 'operand'
            )
        )
    WHERE tvm.version_id = @VersionId
);
