MERGE INTO logix.module_port AS target
USING #temp_module_port AS source
ON target.module_id = (SELECT module_id FROM logix.module WHERE record_hash = source.module_hash)
    AND target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT
    (
        module_id,
        port_number,
        port_type,
        address,
        upstream,
        bus_size,
        record_hash
    )
    VALUES
    (
        (SELECT module_id FROM logix.module WHERE record_hash = source.module_hash),
        source.port_number,
        source.port_type,
        source.address,
        source.upstream,
        source.bus_size,
        source.record_hash
    );
