using System.Data;
using L5Sharp.Core;
using LogixDb.Data.Maps;

namespace LogixDb.Data.Sqlite.Imports;

/// <summary>
/// A class responsible for importing controller data from an L5X file into an SQLite database.
/// Implements element import for <see cref="Controller"/> objects by extracting the single
/// controller instance from the L5X content and mapping it to the database using <see cref="ControllerMap"/>.
/// </summary>
internal class SqliteControllerImport() : SqliteImport<ControllerRecord>(new ControllerMap());