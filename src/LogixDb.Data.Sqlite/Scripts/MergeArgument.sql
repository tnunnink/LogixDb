INSERT INTO argument (instruction_id,
                      argument_index,
                      argument_type,
                      argument_text,
                      record_hash)
SELECT (SELECT instruction_id FROM instruction WHERE record_hash = t.instruction_id),
       t.argument_index,
       t.argument_type,
       t.argument_text,
       t.record_hash
FROM temp_argument t
ON CONFLICT (instruction_id, record_hash) DO NOTHING;
