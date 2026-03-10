using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

internal class SqliteAoiOperandImport() : SqliteImport<AoiOperandRecord>(new AoiOperandMap());