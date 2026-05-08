using System.Data;

namespace LogixDb.Data;

/// <summary>
/// Defines an abstract base class for mapping Logix elements to database table structures.
/// Provides the schema definition for how a specific type of Logix element should be stored in the database,
/// including the table name and column mappings.
/// </summary>
/// <typeparam name="T">The type of Logix element this table map represents must implement ILogixElement.</typeparam>
internal abstract class TableMap<T> where T : class
{
    /// <summary>
    /// Gets the name of the database table that will store the mapped Logix elements.
    /// </summary>
    protected abstract string TableName { get; }

    /// <summary>
    /// Gets the collection of column mappings that define how properties of the Logix element
    /// are mapped to columns in the database table.
    /// </summary>
    protected abstract IReadOnlyList<ColumnMap<T>> Columns { get; }

    /// <summary>
    /// Generates a DataTable representation of the provided records.
    /// The DataTable will have columns corresponding to the table's defined columns
    /// and rows populated with data from the provided records.
    /// </summary>
    /// <param name="records">
    /// A collection of records of type T to populate the DataTable. Each record's
    /// properties are mapped to columns in the DataTable based on the table's column definitions.
    /// </param>
    /// <returns>
    /// A DataTable containing the column structure defined by the table's column definitions
    /// and rows populated with data from the provided records. If the collection is empty,
    /// the DataTable will contain no rows.
    /// </returns>
    public DataTable GenerateTable(IEnumerable<T> records)
    {
        var table = new DataTable(TableName);
        table.Columns.AddRange(Columns.Select(c => new DataColumn(c.Name, c.Type)).ToArray());

        table.BeginLoadData();

        foreach (var record in records)
        {
            var row = table.NewRow();

            foreach (var column in Columns)
            {
                var value = column.Getter(record) ?? DBNull.Value;
                row[column.Name] = value;
            }

            table.Rows.Add(row);
        }

        table.EndLoadData();
        return table;
    }
}