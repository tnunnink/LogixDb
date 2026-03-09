using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// Represents an implementation of the <see cref="SqliteImport{TRecord}"/> class specialized for importing
/// argument data into a SQLite database.
/// </summary>
internal class SqliteArgumentImport() : SqliteImport<ArgumentRecord>(new ArgumentMap());