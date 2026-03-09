using System.Data;
using System.Globalization;
using System.Text;

namespace LogixDb.Data;

/// <summary>
/// Defines an abstract base class for mapping Logix elements to database table structures.
/// Provides the schema definition for how a specific type of Logix element should be stored in the database,
/// including the table name and column mappings.
/// </summary>
/// <typeparam name="T">The type of Logix element this table map represents must implement ILogixElement.</typeparam>
public abstract class TableMap<T> where T : class
{
    /// <summary>
    /// Stores a collection of column mappings that are involved in calculating the hash of a record.
    /// Each column in this list typically has the `IsHashable` property set to true and is used as part of
    /// the hashing process to uniquely identify or validate the integrity of a record.
    /// Initialized lazily and populated when the `ComputeHash` method is invoked.
    /// </summary>
    private List<ColumnMap<T>>? _hashColumns;

    /// <summary>
    /// Gets the name of the database table that will store the mapped Logix elements.
    /// </summary>
    public abstract string TableName { get; }

    /// <summary>
    /// Gets the collection of column mappings that define how properties of the Logix element
    /// are mapped to columns in the database table.
    /// </summary>
    public abstract IReadOnlyList<ColumnMap<T>> Columns { get; }

    /// <summary>
    /// Extracts and transforms Logix elements from the provided snapshot into a collection
    /// of typed records suitable for database storage. This method defines how to query
    /// and convert the snapshot's source data into the specific record type.
    /// </summary>
    /// <param name="snapshot">
    /// The snapshot containing the Logix source data from which records will be extracted.
    /// </param>
    /// <returns>
    /// A collection of typed records of type T extracted from the snapshot. Each record
    /// represents a Logix element converted into the format defined by the table mapping.
    /// </returns>
    public abstract IEnumerable<T> GetRecords(Snapshot snapshot);

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
        table.Columns.AddRange(Columns.Select(c => new DataColumn(c.Name, c.Type.ToType())).ToArray());

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

    /// <summary>
    /// Computes a hash for the given record by iterating over the record's hashable columns
    /// in a deterministic order. The computed hash is used to ensure data integrity and
    /// verify the consistency of the record.
    /// </summary>
    /// <param name="record">
    /// The instance of the record for which the hash will be computed.
    /// The record is expected to match the structure defined by the table's columns.
    /// </param>
    /// <returns>
    /// A string representing the cryptographic hash of the record's hashable fields.
    /// The hash is computed based on the serialized values of the hashable columns.
    /// </returns>
    public string ComputeHash(T record)
    {
        var columns = _hashColumns ??= GetHashableColumns();
        var hashBuilder = new StringBuilder();

        foreach (var column in columns)
        {
            var value = column.Getter(record) ?? DBNull.Value;
            hashBuilder.Append(SerializeField(column.Name, value));
        }

        return hashBuilder.ToString().Hash().ToHexString();
    }

    /// <summary>
    /// Retrieves a list of columns that are marked as hashable within the current table mapping.
    /// The returned list is sorted by the column names in ordinal order.
    /// </summary>
    /// <returns>
    /// A list of hashable columns defined by the table mapping.
    /// Each column in the list is an instance of <see cref="ColumnMap{T}"/> and has its IsHashable property set to true.
    /// </returns>
    private List<ColumnMap<T>> GetHashableColumns()
    {
        return Columns.Where(c => c.IsHashable).OrderBy(c => c.Name, StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// Serializes a column name and its corresponding value into a formatted string representation.
    /// The resulting string includes specific delimiters to separate the name and value for
    /// consistent serialization of database records.
    /// </summary>
    /// <param name="name">The name of the column being serialized.</param>
    /// <param name="value">The value of the column, which can be null or any object type.</param>
    /// <returns>
    /// A string representing the serialized column name and value, formatted with delimiters.
    /// Null values are replaced by a specific placeholder during serialization.
    /// </returns>
    private static string SerializeField(string name, object? value)
    {
        return '\u001E' + name + '\u001F' + FormatValue(value);

        static string FormatValue(object? value)
        {
            return value switch
            {
                null => "\u2400",
                byte[] b => b.ToHexString(),
                string s => s.Replace("\r\n", "\n"),
                IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString() ?? string.Empty
            };
        }
    }
}