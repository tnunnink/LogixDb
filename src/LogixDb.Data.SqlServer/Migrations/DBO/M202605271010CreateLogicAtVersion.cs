using FluentMigrator;
using JetBrains.Annotations;

namespace LogixDb.Data.SqlServer.Migrations.DBO;

[UsedImplicitly]
[Migration(202605271010, "Create logic_at_version function")]
[Tags(TagBehavior.RequireAny, MigrationTag.Logic)]
public class M202605271010CreateLogicAtVersion : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE OR ALTER FUNCTION dbo.logic_at_version (@VersionId INT)
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
                        FROM dbo.rung r
                        JOIN dbo.target_version_map tvm ON r.rung_id = tvm.record_id
                        JOIN dbo.target_component tc ON tvm.component_id = tc.component_id AND tc.component_name = 'rung'
                        JOIN dbo.rung_instruction ri ON r.rung_id = ri.rung_id
                        LEFT JOIN dbo.rung_argument ra ON ri.rung_id = ra.rung_id 
                            AND ri.instruction_index = ra.instruction_index
                        LEFT JOIN dbo.rung_reference rr ON ra.rung_id = rr.rung_id 
                            AND ra.instruction_index = rr.instruction_index 
                            AND ra.argument_index = rr.argument_index
                        LEFT JOIN dbo.operand o ON ri.instruction_key = o.instruction_key 
                            AND ra.argument_index = o.operand_index
                            AND (
                                o.is_native = 1
                                OR EXISTS (
                                    SELECT 1 FROM dbo.target_version_map tvmo 
                                    JOIN dbo.target_component tco ON tvmo.component_id = tco.component_id
                                    WHERE tvmo.record_id = o.operand_id 
                                      AND tvmo.version_id = @VersionId
                                      AND tco.component_name = 'operand'
                                )
                            )
                        WHERE tvm.version_id = @VersionId
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("DROP FUNCTION IF EXISTS dbo.logic_at_version;");
    }
}
