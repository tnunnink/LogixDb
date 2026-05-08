SELECT component_name
FROM component c
         JOIN sqlite_master m ON m.name = c.component_name;