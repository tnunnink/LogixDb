MERGE INTO dbo.module AS target
USING #temp_module AS source
ON target.record_hash = source.record_hash
WHEN NOT MATCHED THEN
    INSERT
    (
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
        config_tag,
        ip_address,
        slot_number,
        record_hash
    )
    VALUES
    (
        source.module_name,
        source.module_description,
        source.catalog_number,
        source.revision,
        source.vendor_id,
        source.product_id,
        source.product_code,
        source.parent_name,
        source.parent_port,
        source.electronic_keying,
        source.is_inhibited,
        source.is_major_fault_enabled,
        source.is_safety_enabled,
        source.config_tag,
        source.ip_address,
        source.slot_number,
        source.record_hash
    );

INSERT INTO dbo.target_version_map
(
    version_id, record_id, component_id
)
SELECT
    @VersionId,
    (SELECT module_id FROM dbo.module WHERE record_hash = t.record_hash),
    (SELECT component_id FROM dbo.target_component WHERE component_name = 'module')
FROM #temp_module t;
