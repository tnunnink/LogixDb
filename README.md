# LogixDb

An ETL tool for managing and automating ingestion of Rockwell Automation Logix Designer ACD/L5X project files
into a relational SQL database schema, enabling workflows such as project analysis, validation,
documentation, change tracking, and versioning.

## Motivation

Rockwell Automation Logix Designer projects are primarily stored in proprietary binary (`.ACD`) or XML-based (`.L5X`) formats. These formats are optimized for the Studio 5000 IDE but are poorly suited for programmatic analysis, fleet-wide querying, or automated validation. Extracting meaningful insights—such as identifying every instance of a specific Add-On Instruction (AOI) version or enforcing naming conventions across hundreds of projects—typically requires manual effort or complex, fragile parsing scripts.

LogixDb automates the extraction and transformation of these files into a normalized, relational SQL schema. By decomposing monolithic project files into granular, deduplicated components (Tags, Rungs, Routines, Programs, etc.), it enables engineers to:

*   **Execute Complex Queries**: Use standard SQL to perform cross-project analysis that is impossible within the native IDE.
*   **Implement Automated Validation**: Run programmatic quality checks and standards enforcement via SQL stored procedures.
*   **Track Version History**: Maintain a high-performance record of project evolution using a content-addressable storage model that minimizes database growth.
*   **Integrate with Tooling**: Connect PLC project data directly to BI tools, reporting engines, and CI/CD pipelines.

## Table of Contents

- [Core Architecture](#core-architecture)
    - [Architectural Pillars](#architectural-pillars)
    - [Technical Benefits](#technical-benefits)
- [Database Schemas](#database-schemas)
    - [Logix Schema (Primary Storage)](#logix-schema-primary-storage)
    - [QA Schema (Validation Framework)](#qa-schema-validation-framework)
- [Tools](#tools)
    - [CLI](#cli)
    - [REST API](#rest-api)
    - [FTAC Monitor Service](#ftac-monitor-service)
- [Installation & Requirements](#installation--requirements)
    - [Prerequisites](#prerequisites)
    - [Setup](#setup)
- [Configuration](#configuration)
- [Database Providers](#database-providers)
- [ACD Conversion](#acd-file-conversion)
- [Troubleshooting & FAQ](#troubleshooting--faq)
- [License](#license)

## Core Architecture

LogixDb is built on a **Content-Addressable Deduplication** model. Rather than storing monolithic snapshots, it
decomposes projects into granular, immutable components (Tags, Rungs, UDTs) identified by a deterministic hash of their
content.

### Architectural Pillars

* **Content-Addressable Storage**: Every component is hashed based on its semantic properties. Identical logic or
  configurations across versions—or even different PLC projects—are stored exactly once.
* **Manifest-Based Versioning**: The `target_version_map` acts as a lean manifest. A "Version" is simply a collection of
  pointers to deduplicated records, allowing a 50MB project to be represented by a few kilobytes of relational mapping.
* **Relationship Stability**: LogixDb uses stable natural keys (e.g., `program_name`, `tag_name`) to maintain
  relationships. This prevents the "ripple effect" of ID updates when logic changes, ensuring joins remain
  high-performance as history grows.

### Technical Benefits

* **Zero-Redundancy**: Eliminates database bloat by ensuring identical rungs and tags are never duplicated.
* **Fleet-Wide Analysis**: Instantly identify where a specific AOI or UDT version is used across all projects by
  querying a single hash or ID.
* **Linear Scalability**: Tracking thousands of versions has a negligible impact on query performance because the
  manifest-based reconstruction avoids deep, duplicated hierarchies.
* **Instant Diffing**: Differences between versions are identified via hash comparisons at the database engine level,
  enabling high-speed change tracking.

## Database Schemas

LogixDb provides a multi-schema architecture to separate core project data from validation and quality assurance
workflows. Users can create and migrate schemas using the built-in CLI tool.

| Schema  | Purpose                                                                                                                                                                                           |
|---------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `logix` | Core schema containing all imported PLC project content (tags, programs, routines, AOIs, etc.). This is the primary storage layer for deduplicated components.                                    |
| `qa`    | Quality assurance schema providing a validation framework for running automated checks against the logix schema. Enables CI-like workflows using stored procedures.                                |

### Schema Ownership and Extensibility

LogixDb fully **owns and manages** the `logix` and `qa` schemas. These schemas are created and maintained by the tool's
migration system, and their structure should not be modified directly by users. Any manual changes to tables, columns,
or constraints in these schemas may be overwritten during future migrations or cause compatibility issues.

However, users are encouraged to **extend LogixDb's functionality** by creating custom tables, views, stored procedures,
and functions that query or analyze data from the `logix` schema. For better organization and to avoid conflicts with
future tool updates, it is strongly recommended to place custom database objects in **user-defined schemas** (e.g.,
`custom`, `reports`, `analytics`) rather than directly in the `logix` or `qa` schemas. This approach ensures a clean
separation between LogixDb's managed infrastructure and your custom business logic.

## Logix Schema (Primary Storage)

The following sections will walk through the Logix schema table design so users have a clear understanding.
Most of these should be intuitive for Rockwell controls engineers.

### Target Tables

The top level tables for the logix schema all start with `target` which comes from the L5X nomenclature
for exports `target name` and `target type`.

| Table                | Purpose                                                                                                                                                                                                                                                  |
|----------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `target`             | Represents a unique PLC project or component asset. Each target has a unique identifier (`target_key`) that serves as the root container for all versions of that asset.                                                                                 |
| `target_version`     | Stores a snapshot of the target at a specific point in time. Each import creates a new version record containing metadata such as import timestamp, software revision, and the compressed source L5X data.                                               |
| `target_version_map` | The manifest table that maps each version to its deduplicated component records. This lean table enables rapid reconstruction of any historical version by linking `version_id` to the physical `record_id` of components like tags, programs, and AOIs. |
| `target_component`   | A lookup table defining the types of components that can be stored (e.g., Tag, Program, Rung, AOI). Used by `target_version_map` to identify which component table a `record_id` belongs to.                                                             |

These top-level tables form the foundation of LogixDb's content-addressable architecture. The `target_version_map` acts
as the bridge between versions and the actual component data. When a new version is imported, LogixDb hashes each
component (Tags, Rungs, Programs, etc.). If an identical component already exists in the database, the manifest simply
references the existing `record_id`. If the component is new or has changed, a new record is inserted into the
appropriate component table (e.g., `tag`, `program`, `routine`) and the manifest is updated to point to it. This design
ensures that unchanged components are stored exactly once, regardless of how many versions reference them.

#### Example Query

To retrieve all tags associated with a specific version:

```sql
SELECT
    t.*
FROM logix.tag t
     JOIN logix.target_version_map tvm ON t.tag_id = tvm.record_id
     JOIN logix.target_component tc ON tvm.component_id = tc.component_id
WHERE tvm.version_id = 42
  AND tc.component_name = 'tag';
```

### Container Tables

Tables like `controller`, `task`, `program`, and `routine` act as organizational containers. They are
deduplicated at the root level, allowing the database to share entire program or routine metadata records across
thousands of versions if they remain unchanged.

| Table        | Description                                                                                                                                                      |
|--------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `controller` | Global controller settings including name, processor type, and revision. Deduplicated across versions when unchanged.                                            |
| `task`       | Task metadata and execution settings including name, type, priority, rate, and watchdog. Deduplicated when task configuration remains unchanged across versions. |
| `program`    | Program-level metadata including type, main routine, fault routine, and parent folder. Deduplicated when program structure remains unchanged.                    |
| `routine`    | Routine metadata including name, type, and container. Shared across versions when routine definition is identical.                                               |

### Definition Tables

User-Defined Types (UDTs) and Add-On Instructions (AOIs) share a similar deduplication strategy due to their composite
nature and structural definitions.

| Table              | Description                                                                                                                                                                                                                                                                   |
|--------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `data_type`        | Root deduplicated record for UDTs. If two versions of a project (or two different projects) have identical UDT definitions, they share the same `data_type_id`. The `record_hash` captures the entire immutable state including all member definitions and nested structures. |
| `data_type_member` | Stores the flattened structural definition of each UDT member. These records are linked to the parent `data_type_id` and include member names, data types, dimensions, and hierarchical relationships for nested structures.                                                  |
| `aoi`              | Root deduplicated record for Add-On Instructions. Similar to `data_type`, the `record_hash` includes the instruction logic, parameter definitions, and local tag structures, ensuring that identical AOI implementations are stored only once.                                |
| `aoi_parameter`    | Stores the parameter and local tag definitions for each AOI. These are linked to the parent `aoi_id` and include usage type (Input, Output, InOut), data types, default values, and descriptions.                                                                             |

Both UDTs and AOIs follow the same content-addressable pattern as other components—unchanged definitions are shared
across versions and projects, while modifications result in new deduplicated records. This approach enables efficient
storage while maintaining complete historical tracking of structural changes.

### Module Tables

Module tables represent I/O modules and their hierarchical configuration within the controller's I/O tree. These tables
follow the same content-addressable deduplication strategy as other components, allowing unchanged module configurations
to be shared across project versions.

| Table               | Description                                                                                                                                                                                                                                                     |
|---------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `module`            | Root deduplicated record for I/O modules including chassis, network adapters, and field devices. The `record_hash` captures the complete module configuration including type, catalog number, vendor, revision, and parent-child relationships in the I/O tree. |
| `module_connection` | Stores the connection parameters and communication settings for each module. These records are linked to the parent `module_id` and include detailed configuration data specific to the module type.                                                            |
| `module_port`       | Stores individual port configurations for modules that support multiple network connections or bus interfaces. Linked to the parent `module_id` and includes port-specific addressing, network type, and connection parameters.                                 |

Module configurations are deduplicated based on their complete state including catalog information, electronic keying
mode, major/minor revision requirements, connection parameters, and hierarchical position within the I/O tree. When a
module configuration remains unchanged between versions, the existing `module_id` is reused in the manifest.

### Tag Tables

The tag structure is the most complex due to its hierarchical nature and split metadata. We use a deterministic
`record_hash` (Config Hash) to represent the immutable state of a tag.

| Table         | Description                                                                                                                                                                                                                              |
|---------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `tag`         | Root deduplicated record. If two versions of a project (or two different projects) have identical tag definitions, they share the same `tag_id`. The `record_hash` captures the entire immutable state including members and properties. |
| `tag_member`  | Stores the flattened structural definition. These are linked to the parent `tag_id`.                                                                                                                                                     |
| `tag_comment` | Stores only explicit overrides at the member level. Pass-through documentation is derived at query time.                                                                                                                                 |
| `tag_value`   | Volatile and version-specific table. It is tied to both `version_id` and `tag_id`, ensuring that even if the structure is shared, the data snapshots remain unique to each project capture.                                              |

### Rung Tables

Logic components (Rungs, Instructions, Arguments) lack natural names and rely on positional identity within a routine.

| Table              | Description                                                                                                                                                                                                                                                                    |
|--------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `rung`             | Uses a `rung_id` (64 bit integer) as a stable relational handle. Deduplicated via `record_hash` which combines content (`rung_text`) and position (`rung_number`). Can contain both program instance rung code and AOI definition code, denoted by the `is_definition` column. |
| `rung_instruction` | Linked to the parent rung via the `rung_id`. Stores granular instruction data including their own hashes for fast logic searching and change detection.                                                                                                                        |
| `rung_argument`    | Linked to the parent rung via the `rung_id`. Stores individual instruction arguments and operands. Includes hashes for efficient change detection.                                                                                                                             |
| `rung_reference`   | Stores cross-references between rungs and the tags or components they interact with. Enables dependency tracking and impact analysis across project logic.                                                                                                                     |

Rungs in LogixDb can represent two distinct types of logic: **program instance code** (rungs within routines executing
in tasks) and **AOI definition code** (the internal logic that defines an Add-On Instruction). The `is_definition`
column distinguishes between these contexts, allowing the same rung deduplication strategy to apply to both executable
program logic and reusable instruction definitions.

### Operand Table

The `operand` table provides metadata about instruction operands, including their semantic role and how they interact
with tag data. This table is particularly useful for identifying **destructive operations**—instructions that modify tag
values rather than simply reading them.

By joining `rung_argument` with the `operand` table on `instruction_key` and `argument_name`, users can filter logic to
find all write operations or destructive references to specific tags. This enables advanced analysis such as impact
assessments, data flow tracking, and validation of read-only constraints.

**Example Query: Find all destructive arguments for a specific tag**

```sql
SELECT
    ra.rung_id,
    ra.instruction_key,
    ra.argument_name,
    ra.argument_value,
    o.operand_type
FROM logix.rung_argument ra
     JOIN logix.operand o ON ra.instruction_key = o.instruction_key
        AND ra.argument_name = o.operand_name
WHERE o.is_destructive = 1
  AND ra.argument_value LIKE '%YourTagName%';
```

## QA Schema (Validation Framework)

The `qa` schema provides a framework for executing automated data validation against the `logix` schema. While conceptually similar to **tSQLt**, LogixDb focuses on **data-level validation** (analyzing the content and logic of the PLC project) rather than schema or SQL object validation. It allows users to define reusable validation rules as SQL stored procedures that accept version-specific parameters and return structured results for analysis and reporting.

Unlike the `logix` schema (which stores immutable project content), the `qa` schema is designed to track validation
execution history and results over time.

| Table               | Description                                                                                                                                                                                                |
|---------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `validation_run`    | Tracks a group of validation executions. Stores the execution metadata (who, when, status) and the input variables (as JSON) used for the run.                                                             |
| `validation_result` | Stores the individual results for each validation procedure executed within a run. Contains success status, a summary message, and detailed JSON data for violations or successes.                         |

### Validation Design

LogixDb uses a **Stored Procedure-based** validation model. Validations are standard SQL procedures that accept a
`qa.variables` table-valued parameter and return a result set matching the `qa.outcome` type.

*   **`qa.variables`**: A key-value table used to pass parameters (like `version_id`) to the validation logic.
*   **`qa.outcome`**: A standard structure for validation results, including `is_success`, `result_message`, and `result_details` (JSON).

### Creating Validations

Use the `qa.create_validation` helper procedure to scaffold a new validation. This ensures the procedure follows the
required contract and places it in a dedicated "validation class" schema (e.g., `naming`, `security`, `standards`).

```sql
-- Create a new validation in the 'standards' class
EXEC qa.create_validation @validation_class = 'standards', @validation_name = 'check_tag_naming';
```

### Anatomy of a Validation

A validation is a stored procedure that follows a specific contract. It receives input variables (like the target
`version_id`) and returns a result set describing any violations.

#### 1. Input Variables (`qa.variables`)
Every validation must accept `@vars qa.variables READONLY`. Use the `qa.get_variable_as_int` helper to safely extract
values:

```sql
DECLARE @version_id INT;
EXEC qa.get_variable_as_int @vars, 'version_id', @version_id OUT;
```

#### 2. Validation Logic
The procedure should query the `logix` schema to find data that violates a specific rule. It is best practice to
gather violating records into a temporary table.

#### 3. Emitting Results (`qa.emit_failure`)
If violations are found, use `qa.emit_failure` to return a structured result. This helper converts a result set into
a JSON payload, which is essential for detailed reporting.

```sql
-- Example: Finding tags that don't follow naming conventions
SELECT tag_name, 'Name must start with TAG_' as reason
INTO #violations
FROM logix.tag t
     JOIN logix.target_version_map tvm ON t.tag_id = tvm.record_id
WHERE tvm.version_id = @version_id
  AND t.tag_name NOT LIKE 'TAG_%';

IF EXISTS (SELECT 1 FROM #violations)
BEGIN
    SELECT * FROM qa.emit_failure(
        'One or more tags do not follow the naming convention.',
        (SELECT * FROM #violations FOR JSON PATH) -- Return failed data as JSON
    );
END
```

### Running Validations

Validations can be executed individually or in batches using the `qa.run_validation` or `qa.run_validations` procedures.
The runner automatically handles creating a `validation_run` record, capturing execution time, and logging results.

**Example: Run all validations for a specific version**

```sql
DECLARE @vars qa.variables;
INSERT INTO @vars (variable_name, variable_value) VALUES ('version_id', '42');

DECLARE @vals qa.validations;
INSERT INTO @vals (validation_name)
SELECT qualified_name FROM qa.list_validations;

-- Run the selected validations
EXEC qa.run_validations @vars = @vars, @vals = @vals, @run_name = 'Nightly Standards Check';
```

Validations can also be run individually:

```sql
DECLARE @vars qa.variables;
INSERT INTO @vars (variable_name, variable_value) VALUES ('version_id', '42');

-- Run a single validation by name
EXEC qa.run_validation @vars = @vars, @validation_name = 'standards.check_tag_naming', @run_name = 'Single Check';
```

#### Inspecting Results (`qa.inspect_result`)

When a validation fails, it often returns a complex JSON payload in the `result_details` column. Use the
`qa.inspect_result` function to flatten this JSON into a table for easier analysis.

```sql
-- View all violations for a specific result ID
SELECT * FROM qa.inspect_result(123);
```

#### Example Query: Retrieve Results for a Run

```sql
SELECT
    vr.validation_name,
    vr.is_success,
    vr.result_message,
    vr.result_details,
    vr.execution_time
FROM qa.validation_result vr
     JOIN qa.validation_run vrun ON vr.run_id = vrun.run_id
WHERE vrun.run_name = 'Nightly Standards Check'
ORDER BY vr.execution_time DESC;
```

## Tools

LogixDb provides multiple interfaces for interacting with the database and automating project ingestion.

### CLI

LogixDb provides an CLI for managing database operations. Use it for importing L5X/ACD files, exporting targets,
and performing database maintenance. See the table below for a complete list of available commands.

| Command        | Description                                                                                                                            |
|:---------------|:---------------------------------------------------------------------------------------------------------------------------------------|
| **`migrate`**  | Runs database migrations to ensure the schema is up to date. Supports selective table creation via `--components`.                     |
| **`import`**   | Ingests an L5X or ACD file into the database. This command automatically deduplicates components and updates the `target_version_map`. |
| **`export`**   | Exports a specific version or target back to an L5X file.                                                                              |
| **`list`**     | Lists all registered targets and their available versions.                                                                             |
| **`prune`**    | Removes metadata for a target that is no longer needed.                                                                                |
| **`truncate`** | Deletes old versions of a specified target before a given date or version number.                                                      |
| **`purge`**    | Permanently deletes a target and its entire history.                                                                                   |
| **`sync`**     | Connects to an online PLC to upload live tag values and creates a new version in the database.                                         |

#### Example Usage

The CLI requires a connection string using the `-c` or `--connection` option. For **SQLite**, this is a file path.
For **SQL Server**, use the format `DatabaseName@ServerHost`.

**Run migrations to ensure the latest database schema**:

```powershell
logixdb migrate -c "C:\Data\Logix.db"
```

**Import an L5X file (SQLite)**:

```powershell
logixdb import -c "C:\Data\Logix.db" -s "C:\Projects\MyProject.L5X" -t "PLC://Main_Controller"
```

**List all versions for a specific target (SQL Server)**:

```powershell
logixdb list -c "LogixDb@localhost" -t "PLC://Main_Controller"
```

**Export a specific version to a file**:

```powershell
logixdb export -c "LogixDb@localhost" -t "PLC://Main_Controller" -v 1 -o "C:\Exports\Backup_v1.L5X"
```

### REST API

LogixDb comes with a built-in Windows service that hosts a lightweight REST API for automated project ingestion.
This allows external tools, CI/CD pipelines, or scripts to upload PLC files to the service, which then processes
and ingests them into the configured database in the background.

#### Endpoint

| Path      | Method | Content-Type          | Description                                                     |
|-----------|--------|-----------------------|-----------------------------------------------------------------|
| `/ingest` | `POST` | `multipart/form-data` | Uploads an L5X or ACD file for background parsing and ingestion |
| `/health` | `GET`  | `application/json`    | Returns the current service status and system time              |

#### Request

The `/ingest` endpoint expects a `multipart/form-data` request with a single `file` field containing the L5X or ACD
source file.

Custom metadata can be associated with an upload by including request headers prefixed with `Logix-`. These headers
will be extracted and stored alongside the target metadata.

Example: `Logix-Target: PLC_A` or `Logix-Environment: Production`.

#### Response

Upon a successful upload, the API returns a `202 Accepted` response with the following JSON structure:

```json
{
  "importId": "guid-of-imported-file",
  "received": "ProjectName",
  "status": "Queued"
}
```

#### Example Usage

Upload a file using `curl`:

```powershell
curl -X POST http://localhost:5088/ingest `
  -F "file=@C:\Projects\MyProject.acd" `
  -H "Logix-Target: MyTarget"
```

### FTAC Monitor Service

The same Windows service that hosts the REST API also includes an optional FTAC (FactoryTalk AssetCentre)
monitoring feature. When enabled, this service automatically monitors a FactoryTalk AssetCentre database for new
versions of `.ACD` files, downloads them, and ingests them into the configured LogixDb.

#### How it Works

1. **Polling**: Background periodically polls AssetCentre database for new asset versions.
2. **Download**: When a new version is detected, the background service downloads the file from the database locally.
3. **Ingestion**: The downloaded file queued for background ingestion into the configured LogixDb database.

## Configuration

To configure the Windows service, update the `LogixConfig` section in `appsettings.json`:

| Setting          | Type       | Default | Description                                                                                                       |
|------------------|------------|---------|-------------------------------------------------------------------------------------------------------------------|
| `DbConnection`   | `String`   | `null`  | The connection string for the LogixDb database (SQLite file path or `DatabaseName@ServerHost` for SQL Server).    |
| `DropPath`       | `String`   | `null`  | The local directory where `.L5X` or `.ACD` files are placed for background ingestion.                             |
| `AcdConverter`   | `String`   | `null`  | Path to a custom CLI tool for converting ACD to L5X. Expected contract: `convert -i <input> -o <output> --force`. |
| `FtacMonitor`    | `Boolean`  | `false` | Enables or disables the FTAC monitoring background services.                                                      |
| `FtacConnection` | `String`   | `null`  | Optional SQL connection string override for the AssetCentre database.                                             |
| `FtacFilters`    | `String[]` | `[]`    | A list of asset name filters (wildcards supported) to limit which assets are monitored.                           |

> [!IMPORTANT]
> The service account running LogixDb must have `SELECT` and `EXECUTE` permissions on the FactoryTalk AssetCentre
> database. By default, the service assumes a local AssetCentre installation with Windows Authentication.

#### Example Configuration

```json
{
  "LogixConfig": {
    "DbConnection": "LogixDb@localhost",
    "DropPath": "C:\\ProgramData\\LogixDb\\Uploads",
    "AcdConverter": "C:\\Tools\\AcdToL5x.exe",
    "FtacMonitor": true,
    "FtacConnection": "Data Source=RemoteServer;Initial Catalog=AssetCentre;Integrated Security=SSPI;",
    "FtacFilters": [
      "Area1*",
      "Line2*",
      "!*Backup*"
    ]
  }
}
```

#### FTAC Asset Name Filtering

The `FtacFilters` configuration allows you to control which ACD files are processed from the FactoryTalk AssetCentre
database. It supports standard wildcards and both **Whitelisting** (inclusion) and **Blacklisting** (exclusion).

#### Supported Wildcards

* `*`: Matches **zero or more** characters (e.g., `*Test*` matches `Test.ACD`, `NewTest.ACD`, and `Test_Final.ACD`).
* `?`: Matches **exactly one** character (e.g., `Line?_Prog` matches `Line1_Prog` and `LineA_Prog`, but not
  `Line12_Prog`).

#### Filtering Rules

1. **Blacklists**: Any filter starting with `!` is a blacklist. If an asset name matches **any** blacklist pattern, it
   is excluded.
2. **Whitelists**: Filters without a `!` prefix are whitelists. If any whitelist patterns are defined, the asset name
   must match **at least one** of them to be included.
3. **Default**: If no filters are provided, all `.ACD` files are processed.

#### Example Filter Configurations

| Filter Pattern                                 | Description                                                                  |
|------------------------------------------------|------------------------------------------------------------------------------|
| `Area1*`                                       | Only process assets that start with "Area1"                                  |
| `Line1*`, `Line2*`, `!*Backup*`                | Process assets from Line 1 or 2, but exclude anything containing "Backup"    |
| `!Test*`, `!*TEMP.ACD`                         | Process all assets except those starting with "Test" or ending in "TEMP.ACD" |
| `Unit?.ACD`                                    | Match "Unit1.ACD" through "Unit9.ACD", but not "Unit10.ACD"                  |
| `*Main*`, `*Safety*`, `!Area51*`, `!*Sandbox*` | Include "Main" or "Safety" assets, but exclude "Area51" and "Sandbox" assets |

## Database Providers

LogixDb currently supports both Microsoft SQL Server and SQLite database providers.

| Provider       | Description                                                                                                                                                                                                                                                                             |
|----------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **SQLite**     | Ideal for single-developer or quick analysis scenarios. Free and open source with no additional server-side software required. Developers can quickly transform PLC projects into SQLite databases on the fly. Generated database files can be queried using any preferred client.      |
| **SQL Server** | Designed for team environments, especially those using version control systems like FTAC, Git, or SVN. Enables centralized data management and supports advanced features such as stored procedures, triggers, tSQLt, and custom tooling for enhanced collaboration and data integrity. |

This tool enables automated ingestion of L5X and ACD files into either database provider.

## ACD File Conversion

LogixDb uses the Rockwell Logix Designer SDK to convert `.ACD` files into `.L5X` so they can be parsed and
ingested. By default, the service uses the SDK on the local machine to perform this conversion. Since spinning up
a headless Studio 5000 instance to save as `.L5X` is a resource-intensive process, this task is handled by the
Windows service in the background as new files are uploaded or detected in version control.

### Custom Converter Executable

To avoid software redistribution and provide flexibility, LogixDb allows users to specify a custom command-line
executable for `.ACD` conversion. If a custom converter is specified, the service will call it instead of the default
SDK-based converter.

The custom converter must support the following CLI arguments:
`convert -i <input_path> -o <output_path> --force`

> [!NOTE]
> This capability is provided to allow users to integrate their own conversion tools and to ensure that LogixDb
> does not redistribute proprietary Rockwell Automation software.

## Installation & Requirements

### Prerequisites

- Windows 10 or later
- PowerShell 5.1 or later (for automated installation)
- Rockwell Automation Software (Optional or use case dependent)
    - **Logix Designer / Studio 5000**: Required on the machine performing conversions if processing `.ACD` files.
    - **Rockwell Logix Designer SDK**: Used for `.ACD` file conversion by default.
    - **FactoryTalk AssetCentre**: Required if using the `FtacMonitorService` to automatically pull files from an
      AssetCentre database. This could be installed on remote machine as well.

### Setup

1. Download the latest release ZIP from [releases](https://github.com/tnunnink/LogixDb/releases)
2. Extract the ZIP to a temporary location
3. Open PowerShell as an Administrator
4. Navigate to the extracted directory
5. Unblock the PowerShell script:
   ```powershell
   Unblock-File -Path .\Setup.ps1
   ```
6. Run the installation script:
   ```powershell
   .\Setup.ps1
   ```

The setup script automates the following steps:

- **Service Deployment**: Stops any existing `LogixDb` service and deploys files to `C:\Program Files\LogixDb`.
- **SQL Permissions**: Checks for a local FactoryTalk AssetCentre database and seeds the necessary `SELECT` and
  `EXECUTE`
  permissions for the `NT SERVICE\LogixDb` service account.
- **Service Configuration**: Creates or updates the `LogixDb` Windows Service to run automatically.
- **System PATH**: Adds the installation directory to the system `PATH`, making the `logixdb` CLI available globally.
- **Service Startup**: Starts the `LogixDb` service to begin monitoring or hosting the Ingestion API.

> [!IMPORTANT]
> The setup script does **not** automatically migrate existing LogixDb databases. If you are upgrading or
> reinstalling, you must manually run the `logixdb migrate` command to ensure the schema is up to date
> before re-enabling or relying on the service. Check the Windows Event Viewer for errors to ensure no issues with
> database connection/validation.

### Service Management

The `LogixDb` service runs as a Windows Service. You can manage its lifecycle using the following PowerShell commands as
an Administrator:

```powershell
# View service status
Get-Service -Name LogixDb

# Restart the service (required after appsettings.json changes)
Restart-Service -Name LogixDb -Force

# View service properties and start type
Get-Service -Name LogixDb | Select-Object -Property Name, Status, StartType
```

By default, the service is installed in `C:\Program Files\LogixDb` and runs under the `NT SERVICE\LogixDb` account. If
you need to access remote network shares or specific AssetCentre instances, you may need to change the service account
via `services.msc`.

### Logging & Diagnostics

LogixDb uses a dual-logging approach to track both service-level and import-level activity:

* **Service Logs**: Critical service errors and startup/shutdown events are logged to the **Windows Event Log** under
  the "Application" source with the source name "LogixDb".
* **Import Tracking**: All import-related informational messages, warnings, and errors are stored in the target database
  within two dedicated tables:
    * **`import`**: Tracks the overall status and metadata for each import operation (file name, source type,
      timestamps, status).
    * **`import_log`**: Contains granular log entries for each import, including severity level (Info, Warning, Error),
      messages, and exception details.
* **Health Check**: You can verify the service is running by visiting `http://localhost:5088/health` in your browser.

To review import history and troubleshoot failed imports, query the `import` and `import_log` tables directly in your
LogixDb database. This provides a complete audit trail of all ingestion attempts, including detailed error messages and
stack traces when applicable.

### Troubleshooting

| Issue                             | Probable Cause                                            | Remedy                                                                                           |
|:----------------------------------|:----------------------------------------------------------|:-------------------------------------------------------------------------------------------------|
| **FTAC Polling returns 0 assets** | Filter is too restrictive or permissions issue.           | Check `FtacFilters` and ensure the service account has `SELECT` on the AC database.              |
| **ACD Conversion Fails**          | Studio 5000 version mismatch or licensing.                | Ensure the correct version of Studio 5000 is installed and licensed on the service machine.      |
| **Migration Errors**              | Database file is locked or user lacks schema permissions. | Stop the service before running manual migrations; ensure the user has `db_owner` or equivalent. |
| **CLI "Target Not Found"**        | Target key mismatch.                                      | Use `logixdb list` to see existing target keys; keys are case-sensitive.                         |

## Feedback

Feedback, bug reports, and feature requests are welcome. Please use
the [GitHub Issues](https://github.com/tnunnink/LogixDb/issues) page to share your thoughts or report problems.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for full details.
