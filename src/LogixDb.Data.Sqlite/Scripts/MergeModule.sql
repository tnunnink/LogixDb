INSERT OR IGNORE INTO module (module_name,
                              module_description,
                              catalog_number,
                              revision,
                              vendor_id,
                              product_id,
                              product_code,
                              parent_name,
                              parent_port,
                              electronic_keying,
                              is_inhibited,
                              is_major_fault_enabled,
                              is_safety_enabled,
                              ip_address,
                              slot_number,
                              record_hash)
SELECT t.module_name,
       t.module_description,
       t.catalog_number,
       t.revision,
       t.vendor_id,
       t.product_id,
       t.product_code,
       t.parent_name,
       t.parent_port,
       t.electronic_keying,
       t.is_inhibited,
       t.is_major_fault_enabled,
       t.is_safety_enabled,
       t.ip_address,
       t.slot_number,
       t.record_hash
FROM temp_module t;

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT module_id FROM module WHERE record_hash = t.record_hash),
       (SELECT component_id FROM component WHERE component_name = 'module')
FROM temp_module t;
