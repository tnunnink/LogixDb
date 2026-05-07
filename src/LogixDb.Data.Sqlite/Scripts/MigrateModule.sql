INSERT INTO module (module_id,
                    parent_id,
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
                    record_hash,
                    source_hash)
SELECT module_id,
       parent_id,
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
       record_hash,
       source_hash
FROM temp_module
ON CONFLICT (source_hash) DO NOTHING;

INSERT INTO target_version_map (version_id, component_id, component_type)
SELECT @VersionId, (SELECT module_id FROM module WHERE source_hash = t.source_hash), 'module'
FROM temp_module t;
