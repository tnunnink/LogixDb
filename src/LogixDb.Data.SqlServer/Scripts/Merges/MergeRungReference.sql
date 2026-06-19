MERGE INTO logix.rung_reference AS target
USING #temp_rung_reference AS source
ON target.rung_id = (SELECT rung_id FROM logix.rung WHERE record_hash = source.rung_hash)
WHEN NOT MATCHED THEN
    INSERT
    (
        rung_id,
        instruction_index,
        argument_index,
        reference_name
    )
    VALUES
    (
        (SELECT rung_id FROM logix.rung WHERE record_hash = source.rung_hash),
        source.instruction_index,
        source.argument_index,
        source.reference_name
    );