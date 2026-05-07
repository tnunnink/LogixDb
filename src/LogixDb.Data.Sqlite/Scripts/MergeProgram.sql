-- Step 1: Build the hierarchy depth and map temporary Task IDs
WITH RECURSIVE program_hierarchy AS (
    -- Anchor: Root programs (no parent folder)
    SELECT
        t.program_id as temp_id,
        t.folder_id as temp_parent_id,
        t.source_hash,
        0 as depth
    FROM temp_program t
    WHERE t.folder_id IS NULL

    UNION ALL

    -- Recursive Step: Child programs
    SELECT
        t.program_id,
        t.folder_id,
        t.source_hash,
        ph.depth + 1
    FROM temp_program t
             INNER JOIN program_hierarchy ph ON t.folder_id = ph.temp_id
),
ordered_program AS (
   -- Combine hierarchy data with original temp data
   SELECT
       (SELECT task_id FROM task WHERE source_hash = (SELECT source_hash FROM temp_task WHERE task_id = t.task_id)) actual_task_id,
       h.depth,
       t.*
   FROM temp_program t
            JOIN program_hierarchy h ON t.program_id = h.temp_id
)

-- Step 2: Insert programs in topological order (parents first)
INSERT INTO program (program_id,
                     task_id,
                     folder_id,
                     program_name,
                     program_description,
                     program_type,
                     main_routine,
                     fault_routine,
                     is_disabled,
                     is_folder,
                     has_test_edits,
                     record_hash,
                     source_hash)
SELECT
    op.program_id,
    op.actual_task_id,
    -- Resolve the actual folder_id by looking up the persisted record of the parent
    (SELECT program_id FROM program WHERE source_hash = (SELECT source_hash FROM temp_program WHERE program_id = op.folder_id)),
    op.program_name,
    op.program_description,
    op.program_type,
    op.main_routine,
    op.fault_routine,
    op.is_disabled,
    op.is_folder,
    op.has_test_edits,
    op.record_hash,
    op.source_hash
FROM ordered_program op
ORDER BY op.depth
ON CONFLICT (source_hash, task_id, folder_id) DO NOTHING;

-- Note: If multiple folders have the same program source_hash, we'd need folder_id resolution here too

-- Step 3: Link this version to the correct program records
INSERT INTO target_version_map (version_id, component_id, component_type)
SELECT
    @VersionId,
    (SELECT program_id FROM program
     WHERE source_hash = t.source_hash
       AND (task_id = (SELECT task_id FROM task WHERE source_hash = (SELECT source_hash FROM temp_task WHERE task_id = t.task_id))
         OR (task_id IS NULL AND t.task_id IS NULL))
        -- Note: If multiple folders have the same program source_hash, we'd need folder_id resolution here too
    ),
    'program'
FROM temp_program t;