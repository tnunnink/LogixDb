using LogixDb.Data.Maps;

namespace LogixDb.Data.SqlServer.Imports;

internal class SqlServerAoiOperandImport() : SqlServerImport<AoiOperandRecord>(new AoiOperandMap());