# LogixDb

A tool for managing and automating ingestion of Rockwell Automation Logix Designer ACD/L5X project files
into a structured and transparent SQL database schema, enabling workflows such as project analysis, validation,
documentation, change tracking, and versioning.

## Motivation

Analyzing and extracting data from Rockwell PLC projects is often slow and manual. Without opening Studio 5000, there is
no straightforward way to centrally manage or review code across multiple projects. For system integrators and
developers, tasks like comparing configurations, validating logic versions, or bulk-extracting data remain difficult.

LogixDb was built to make PLC code analysis and data extraction developer-friendly. By parsing PLC files into a
structured SQL schema, it enables developers and controls engineers to leverage the power of SQL to write custom
queries, views, and procedures for project analysis, validation, and documentation.

## Overview

LogixDb currently offers a few tools for users to work with.

### CLI

Interactive command-line tool for managing database operations. Use it for importing L5X/ACD files, exporting snapshots,
and performing database maintenance. See the table below for a complete list of available commands.

| Command     | Description                                                         |
|-------------|---------------------------------------------------------------------|
| **migrate** | Runs migrations to create and/or ensure the latest database schema  |
| **import**  | Imports an L5X or ACD file as a new snapshot into the database      |
| **list**    | Lists all snapshots, optionally filtered by target key              |
| **export**  | Exports a snapshot to an L5X file by target or ID                   |
| **prune**   | Delete snapshots by ID, date, or target                             |
| **purge**   | Purges all data from the database while preserving the schema       |
| **drop**    | Drops the entire database, permanently deleting all tables and data |

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

**List all snapshots for a specific target (SQL Server)**:

```powershell
logixdb list -c "LogixDb@localhost" -t "PLC://Main_Controller"
```

**Export the latest snapshot to a file**:

```powershell
logixdb export -c "LogixDb@localhost" -t "PLC://Main_Controller" -o "C:\Exports\Backup.L5X"
```

**Prune snapshots older than a specific date**:

```powershell
logixdb prune -c "LogixDb@localhost" --before "2024-01-01"
```

### Ingestion API Endpoint

The Windows service hosts a lightweight REST API for automated project ingestion. This allows external tools,
CI/CD pipelines, or scripts to upload PLC files to the service, which then processes and ingests them into the
configured database in the background.

#### Endpoint

| Path      | Method | Content-Type          | Description                                                     |
|-----------|--------|-----------------------|-----------------------------------------------------------------|
| `/ingest` | `POST` | `multipart/form-data` | Uploads an L5X or ACD file for background parsing and ingestion |
| `/health` | `GET`  | `application/json`    | Returns the current service status and system time              |

#### Request

The `/ingest` endpoint expects a `multipart/form-data` request with a single `file` field containing the L5X or ACD
source file.

Custom metadata can be associated with an upload by including request headers prefixed with `Logix-`. These headers
will be extracted and stored alongside the snapshot metadata.

Example: `Logix-Target: PLC_A` or `Logix-Environment: Production`.

#### Response

Upon a successful upload, the API returns a `202 Accepted` response with the following JSON structure:

```json
{
  "traceId": "guid-of-upload",
  "received": "ProjectName.ACD",
  "status": "Queued"
}
```

#### Example Usage

Upload a file using `curl`:

```powershell
curl -X POST http://localhost:5000/ingest `
  -F "file=@C:\Projects\MyProject.acd" `
  -H "Logix-Target: MyTarget"
```

### FTAC Monitor Service

The Windows service includes an optional FTAC (FactoryTalk AssetCentre) monitoring feature. When enabled, this service
automatically monitors a FactoryTalk AssetCentre database for new versions of `.ACD` files, downloads them, and
ingests them into the configured LogixDb.

#### How it Works

1. **Polling**: The `FtacMonitorService` monitors the AssetCentre database for new asset versions.
2. **Download**: When a new version is detected, the `FtacDownloadService` retrieves the file from the database.
3. **Ingestion**: The downloaded file is placed in the configured `DropPath` and queued for background ingestion.

#### Configuration

To enable FTAC monitoring, update the `LogixConfig` section in the service's `appsettings.json`:

| Setting          | Type       | Default | Description                                                                             |
|------------------|------------|---------|-----------------------------------------------------------------------------------------|
| `FtacMonitor`    | `Boolean`  | `false` | Enables or disables the FTAC monitoring background services.                            |
| `FtacConnection` | `String`   | `null`  | Optional SQL connection string override for the AssetCentre database.                   |
| `FtacFilters`    | `String[]` | `[]`    | A list of asset name filters (wildcards supported) to limit which assets are monitored. |

> [!IMPORTANT]
> The service account running LogixDb must have `SELECT` and `EXECUTE` permissions on the FactoryTalk AssetCentre
> database. By default, the service assumes a local AssetCentre installation with Windows Authentication.

#### Example Configuration

```json
{
  "LogixConfig": {
    "FtacMonitor": true,
    "FtacFilters": [
      "Production\\*",
      "Area_01\\*"
    ]
  }
}
```

## Database Providers

This tool currently supports both Microsoft SQL Server and SQLite database providers.

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

#### Configuration

To configure a custom converter, update the `LogixConfig` section in the service's `appsettings.json`:

| Setting        | Type     | Default | Description                                                                        |
|----------------|----------|---------|------------------------------------------------------------------------------------|
| `AcdConverter` | `String` | `null`  | The full path to a custom command-line executable for `.ACD` to `.L5X` conversion. |

#### Example Configuration

```json
{
  "LogixConfig": {
    "AcdConverter": "C:\\Tools\\MyCustomConverter.exe"
  }
}
```

> [!NOTE]
> This capability is provided to allow users to integrate their own conversion tools and to ensure that LogixDb
> does not redistribute proprietary Rockwell Automation software.

## Installation

LogixDb is distributed as a single ZIP package containing self-contained executables for both the CLI tool and the
Windows service. No .NET runtime installation is required.

### Prerequisites

- Windows 10 or later
- PowerShell 5.1 or later (for automated installation)
- Rockwell Logix Designer SDK (required for ACD file conversion)

### Quick Install (Recommended)

1. Download the latest release ZIP from the [releases page](https://github.com/tnunnink/LogixDb/releases)
2. Extract the ZIP to a temporary location
3. Open PowerShell as an Administrator
4. Navigate to the extracted directory
5. Unblock the PowerShell script:
   ```powershell
   Unblock-File -Path .\Setup-LogixDb.ps1
   ```
6. Run the installation script:
   ```powershell
   .\Setup-LogixDb.ps1
   ```

The setup script automates the following steps:

- **Service Deployment**: Stops any existing `LogixDb` service and deploys files to `C:\Program Files\LogixDb`.
- **SQL Permissions**: Checks for a local FactoryTalk AssetCentre database and seeds the necessary `SELECT` and`EXECUTE`
  permissions for the `NT SERVICE\LogixDb` service account.
- **Service Configuration**: Creates or updates the `LogixDb` Windows Service to run automatically.
- **System PATH**: Adds the installation directory to the system `PATH`, making the `logixdb` CLI available globally.
- **Service Startup**: Starts the `LogixDb` service to begin monitoring or hosting the Ingestion API.

## Database Schema

LogixDb uses a snapshot-based relational schema to store PLC project data. This structure allows for version
tracking and historical analysis of changes across different imports of the same PLC project.

### Core Architecture

The schema is organized around three primary levels:

1. **Target**: Represents a unique asset or project (e.g., `PLC://Main_Controller`).
2. **Snapshot**: A specific version of a target, created during an `import` operation. It contains metadata about
   the import (date, user, machine) and the original source file (`source_data`).
3. **Entities**: The granular components of the Logix project (Tags, Routines, Rungs, etc.) associated with a
   specific `snapshot_id`.

```mermaid

    TARGET ||--o{ SNAPSHOT : contains
    SNAPSHOT ||--o{ CONTROLLER : "1:1"
    SNAPSHOT ||--o{ TAG : contains
    SNAPSHOT ||--o{ PROGRAM : contains
    SNAPSHOT ||--o{ TASK : contains
    SNAPSHOT ||--o{ AOI : contains
    SNAPSHOT ||--o{ DATA_TYPE : contains
    SNAPSHOT ||--o{ MODULE : contains

    PROGRAM ||--o{ ROUTINE : defines
    ROUTINE ||--o{ RUNG : contains
    RUNG ||--o{ INSTRUCTION : has
    INSTRUCTION ||--o{ ARGUMENT : takes

    AOI ||--o{ AOI_PARAMETER : defines
    DATA_TYPE ||--o{ DATA_TYPE_MEMBER : defines
```

### Primary Tables

| Table              | Description                                                                                    |
|--------------------|------------------------------------------------------------------------------------------------|
| `target`           | Stores unique target keys for identifying different PLC projects.                              |
| `snapshot`         | Links an import to a target. Stores the raw source file and import metadata.                   |
| `controller`       | Global controller settings (name, processor type, revision, etc.).                             |
| `data_type`        | User-defined and system-defined data type definitions.                                         |
| `data_type_member` | Individual members of a data type, including their name, data type, and dimensions.            |
| `aoi`              | Add-On Instruction definitions, including revision and creation metadata.                      |
| `aoi_parameter`    | Parameters and local tags for AOIs, including usage (Input, Output, InOut) and default values. |
| `module`           | IO configuration and module properties (catalog number, slot, IP address).                     |
| `tag`              | All controller and program scope tags, including names, types, values, and descriptions.       |
| `task`             | Task metadata and execution settings (name, type, priority, rate, watchdog).                   |
| `program`          | Program-level metadata (type, main routine, fault routine, parent folder).                     |
| `routine`          | Routine metadata (name, type, container).                                                      |
| `rung`             | Individual rungs of ladder logic, including the original L5X code and rung comments.           |
| `instruction`      | Granular instruction data extracted from rungs (name, text, destructive bit).                  |
| `argument`         | Individual instruction arguments and operands (tag name, constant value, index).               |

### Relationships

Most tables include a `snapshot_id` column that serves as a foreign key to the `snapshot` table. This allows you to
query all components of a specific project version using a single ID. For example, to find all tags for a specific
snapshot:

```sql
SELECT *
FROM tag
WHERE snapshot_id = 42;
```

### Data Integrity

Each entity record includes a `record_hash` (calculated from its hashable fields). This is used during the ingestion
process to ensure data consistency and can be leveraged for identifying changes between snapshots at a granular level.

## Feedback

Feedback, bug reports, and feature requests are welcome. Please use
the [GitHub Issues](https://github.com/tnunnink/LogixDb/issues) page to share your thoughts or report problems.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for full details.
