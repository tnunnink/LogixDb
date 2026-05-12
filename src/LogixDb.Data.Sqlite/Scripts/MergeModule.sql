INSERT INTO module (parent_id,
                    module_name,
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
SELECT -1,
       t.module_name,
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
FROM temp_module t
         LEFT JOIN module m ON m.record_hash = t.record_hash
    AND m.parent_id = (SELECT module_id FROM module where record_hash IS t.parent_id)
WHERE m.module_id IS NULL;

/*UPDATE module
SET parent_id =  
FROM temp_module t
WHERE parent_id = -1;*/

/*INSERT OR IGNORE INTO module (parent_id,
                              module_name,
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
SELECT (SELECT module_id FROM module WHERE record_hash = t.parent_id),
       t.module_name,
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
FROM temp_module t;*/

INSERT INTO target_version_map (version_id, record_id, component_id)
SELECT @VersionId,
       (SELECT module_id
        FROM module
        WHERE record_hash = t.record_hash
          AND parent_id IS (SELECT module_id FROM module WHERE record_hash = t.parent_id)),
       (SELECT component_id FROM component WHERE component_name = 'module')
FROM temp_module t;
