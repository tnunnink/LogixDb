INSERT OR IGNORE INTO module_port
(
    module_id,
    port_number,
    port_type,
    address,
    upstream,
    bus_size,
    record_hash
)
SELECT
        (SELECT module_id FROM module WHERE record_hash = t.module_hash),
        t.port_number,
        t.port_type,
        t.address,
        t.upstream,
        t.bus_size,
        t.record_hash
FROM temp_module_port t;
