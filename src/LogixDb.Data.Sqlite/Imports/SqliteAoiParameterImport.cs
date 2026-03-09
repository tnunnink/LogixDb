using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// A class responsible for importing AOI (Add-On Instruction) parameter data from an L5X file into an SQLite database.
/// Implements element import for <see cref="Parameter"/> objects by extracting all parameters from all
/// Add-On Instructions in the L5X content and mapping them to the database using <see cref="AoiParameterMap"/>.
/// </summary>
internal class SqliteAoiParameterImport() : SqliteImport<AoiParameterRecord>(new AoiParameterMap());