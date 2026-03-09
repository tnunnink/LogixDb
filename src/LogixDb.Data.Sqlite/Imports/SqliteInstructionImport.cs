using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Represents a specialized import process for SQLite that handles the extraction and
/// transformation of Logix Rung Instructions from a data snapshot into a database-compatible
/// data table using a predefined mapping strategy.
/// </summary>
/// <remarks>
/// This class inherits from <see cref="SqliteImport{TRecord}"/> with the specific use case of
/// importing Instruction data. The import process leverages the <see cref="InstructionMap"/> to
/// define schema mappings and transformations for Instructions.
/// The main purpose of this class is to extract all Rung instructions associated with a given
/// snapshot, compute metadata such as hashes, and populate a DataTable with the necessary
/// fields for database insertion using the provided map schema.
/// </remarks>
/// <example>
/// This class is not designed for direct instantiation by consumers. Instead, it is used
/// as part of the broader import processing pipeline invoked within a LogixDb session.
/// </example>
internal class SqliteInstructionImport() : SqliteImport<InstructionRecord>(new InstructionMap());