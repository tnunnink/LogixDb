# LogixDb

A simple command line tool that converts Rockwell Automation Logix projects into a SQL database
for analysis, change tracking, and versioning.

## Overview

LogixDb imports Rockwell L5X files into a structured SQL database, enabling:

- **Analysis**: Query and analyze tags, logic, and configurations using SQL queries
- **Comparison**: Compare snapshots of PLC programs to identify differences across versions or plants/sites
- **Documentation**: Generate reports and documentation from controller data
- **Change Management**: Maintain a history of program modifications to projects or assets
- **Tool Development**: Build any tools on top of the database to supplement automation development workflows.

Currently, supports SQLite and SQL Server.

## Installation

### Standalone Executable

Pre-built executables are available on the [Releases](https://github.com/yourusername/logixdb/releases) page for use
without a .NET installation.

1. Download the appropriate executable (`logixdb-win-x64.zip`).
2. Extract the contents to a folder of your choice (e.g., `C:\Program Files\LogixDb`).
3. (Optional) Add the folder to your system's PATH environment variable to run `logixdb` from any command prompt.

### .NET Tool

LogixDb is also available as a .NET global tool via NuGet package:

1. Download the NuGet package (e.g., `logixdb.1.0.0.nupkg`) from
   the [Releases](https://github.com/yourusername/logixdb/releases) page.
2. Install it using the .NET CLI by adding the local package as a source:

   ```bash
   dotnet tool install -g LogixDb --add-source /path/to/downloaded/package
   ```

   Replace `/path/to/downloaded/package` with the actual path to the downloaded `.nupkg` file.

Once installed, the `logixdb` command is available globally.

## Commands

All commands require a `--connection` (`-c`) argument specifying either a file path
(SQLite) or a `server/database` string (SQL Server). The provider is inferred
automatically, or can be set explicitly with `--provider`.

#### `migrate`

Creates or migrates the database schema to the latest version.

```bash
logixdb migrate -c ./mydb.db
logixdb migrate -c localhost/Logix
```

### `import`

Imports an L5X file as a new snapshot into the database.

```bash
logixdb import -c ./mydb.db -s ./MyController.L5X
logixdb import -c ./mydb.db -s ./MyController.L5X --action ReplaceLatest
logixdb import -c ./mydb.db -s ./MyController.L5X --target "controller://PlantA"
```

| Option     | Short | Description                                            |
|------------|-------|--------------------------------------------------------|
| `--source` | `-s`  | Path to the L5X file *(required)*                      |
| `--target` | `-t`  | Target key override (if default key is not desired)    |
| `--action` | `-a`  | `Append` *(default)*, `ReplaceLatest`, or `ReplaceAll` |

### `list`

Lists all snapshots in the database, optionally filtered by target.

```bash
logixdb list -c ./mydb.db
logixdb list -c ./mydb.db --target "controller://PlantA"
```

### `export`

Exports a snapshot back to an L5X file.

```bash
logixdb export -c ./mydb.db --target "controller://PlantA"
logixdb export -c ./mydb.db --id 42 --output ./output.L5X
```

### `prune`

Deletes snapshots by ID, date, or target.

```bash
logixdb prune -c ./mydb.db --id 42
logixdb prune -c ./mydb.db --latest --target "controller://PlantA"
logixdb prune -c ./mydb.db --before 2025-01-01
logixdb prune -c ./mydb.db --before 2025-01-01 --target "controller://PlantA"
```

### `purge`

Removes all data from the database while preserving the schema.

```bash
logixdb purge -c ./mydb.db
```

### `drop`

Permanently drops the entire database. Prompts for confirmation.

```bash
logixdb drop -c ./mydb.db
```

### Global Options (all commands)

| Option         | Short | Description                                                       |
|----------------|-------|-------------------------------------------------------------------|
| `--connection` | `-c`  | File path (SQLite) or `server/database` (SQL Server) *(required)* |
| `--provider`   | `-p`  | `Sqlite` or `SqlServer` — inferred if not specified               |
| `--user`       |       | SQL Server username                                               |
| `--password`   |       | SQL Server password                                               |
| `--port`       |       | SQL Server port (default: `1433`)                                 |
| `--encrypt`    |       | Enable connection encryption                                      |
| `--trust`      |       | Trust server certificate without validation                       |

---

## Schema

Each import creates a **snapshot** tied to a **target** (a specific project or
asset). All data tables reference a snapshot, so you can query across multiple
imports and track changes over time.

### `target`

Represents a unique PLC project or asset being tracked.

| Column       | Type        | Description                          |
|--------------|-------------|--------------------------------------|
| `target_id`  | int PK      | Auto-increment primary key           |
| `target_key` | string(128) | Unique identifier for the target     |
| `created_on` | datetime    | When the target was first registered |

### `snapshot`

A single import of an L5X file associated with a target.

| Column              | Type        | Description                            |
|---------------------|-------------|----------------------------------------|
| `snapshot_id`       | int PK      | Auto-increment primary key             |
| `target_id`         | int FK      | Reference to `target`                  |
| `target_type`       | string(128) | Type of the target (e.g. `controller`) |
| `target_name`       | string(128) | Name of the target                     |
| `is_partial`        | bool        | Whether this is a partial export       |
| `schema_revision`   | string(16)  | L5X schema revision                    |
| `software_revision` | string(16)  | Studio 5000 software revision          |
| `export_date`       | datetime    | When the L5X was exported from Studio  |
| `import_date`       | datetime    | When the snapshot was imported         |
| `import_user`       | string(64)  | User who ran the import                |
| `import_machine`    | string(64)  | Machine the import was run from        |
| `source_hash`       | binary(16)  | MD5 hash of the source file            |
| `source_data`       | binary      | Compressed source L5X                  |

### `controller`

Top-level controller configuration from the project.

| Column                  | Type        |
|-------------------------|-------------|
| `controller_id`         | int PK      |
| `snapshot_id`           | int FK      |
| `controller_name`       | string(128) |
| `processor`             | string(128) |
| `revision`              | string(32)  |
| `project_creation_date` | datetime    |
| `last_modified_date`    | datetime    |
| `comm_path`             | string(128) |
| `record_hash`           | binary(16)  |

### `tag`

All tags across controller and program scopes, flattened to individual members.

| Column            | Type        | Description                             |
|-------------------|-------------|-----------------------------------------|
| `tag_id`          | int PK      |                                         |
| `snapshot_id`     | int FK      |                                         |
| `container_name`  | string(128) | Scope (controller name or program name) |
| `tag_name`        | string(256) | Full dot-notation tag path              |
| `base_name`       | string(128) | Root tag name                           |
| `parent_name`     | string(256) | Parent tag path (for nested members)    |
| `member_name`     | string(128) | Leaf member name                        |
| `tag_value`       | string(256) |                                         |
| `data_type`       | string(128) |                                         |
| `description`     | string(512) |                                         |
| `external_access` | string(32)  |                                         |
| `constant`        | bool        |                                         |

### `data_type` / `data_type_member`

User-defined data types and their members.

### `aoi` / `aoi_parameter`

Add-On Instructions and their parameters.

### `task` / `program` / `routine` / `rung`

The full program structure — tasks contain programs, programs contain routines,
routines contain rungs. Rung `code` stores the raw ladder logic text.

### `module`

I/O module configuration from the controller's I/O tree.

## Feedback

We welcome feedback, bug reports, and feature requests! Please visit
our [GitHub Issues](https://github.com/yourusername/logixdb/issues) page to share your thoughts or report problems. For
questions or discussions, feel free to start a discussion in
the [GitHub Discussions](https://github.com/yourusername/logixdb/discussions) section.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for full details.