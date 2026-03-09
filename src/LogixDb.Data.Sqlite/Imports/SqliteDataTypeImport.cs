using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Represents a class for importing data type data into a SQLite database.
/// </summary>
/// <remarks>
/// This class provides functionality to process and import data types into a SQLite database
/// by using a specific set of preconfigured SQL commands and mappings. It works in
/// conjunction with a parent transaction to ensure atomic operations are performed safely.
/// </remarks>
internal class SqliteDataTypeImport() : SqliteImport<DataTypeRecord>(new DataTypeMap());