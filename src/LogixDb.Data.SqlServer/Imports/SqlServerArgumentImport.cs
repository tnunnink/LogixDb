using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;

/// <summary>
/// Imports instruction argument data from L5X snapshots into SQL Server database.
/// </summary>
/// <remarks>
/// This importer extracts all instruction arguments from rungs within a snapshot,
/// flattens the hierarchical structure (Rung -> Instruction -> Argument), and
/// generates a data table for bulk import. Each argument is linked to its parent
/// instruction via computed hash values and includes its position index.
/// </remarks>
internal class SqlServerArgumentImport() : SqlServerImport<ArgumentRecord>(new ArgumentMap());