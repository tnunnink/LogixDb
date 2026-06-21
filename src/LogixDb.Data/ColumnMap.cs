using L5Sharp.Core;

namespace LogixDb.Data;

/// <summary>
/// Represents a mapping configuration between a Logix element property and a database column.
/// This record defines how to extract data from an element of type <typeparamref name="T"/>
/// and map it to a specific database column with the appropriate type and constraints.
/// </summary>
/// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
public sealed record ColumnMap<T> where T : class
{
    /// <summary>
    /// Gets or sets the name of the database column to which a property of <typeparamref name="T"/> is mapped.
    /// This represents the column's identifier in the database schema and serves as a key for data mapping.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the database column type to which a property of <typeparamref name="T"/> is mapped.
    /// This defines the data type representation for the column in the database schema, used for type-specific operations.
    /// </summary>
    public required Type Type { get; init; }

    /// <summary>
    /// Gets the function used to extract the column value from an instance of <typeparamref name="T"/>.
    /// This function is invoked during data import operations to retrieve the value to be stored in the database.
    /// </summary>
    public required Func<T, object?> Getter { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this column should be included in hash calculations.
    /// When true, the column value contributes to the computed record hash; when false, it is excluded.
    /// This is typically false for hash columns themselves to avoid circular dependencies.
    /// </summary>
    public required bool Hashable { get; init; }

    /// <summary>
    /// Creates a new column map for a string-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the string value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the string property and column name.</returns>
    public static ColumnMap<T> For(Func<T, string?> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(string),
            Getter = getter,
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for a boolean-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the boolean value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the boolean property and column name.</returns>
    public static ColumnMap<T> For(Func<T, bool> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(bool),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for a nullable boolean-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the nullable boolean value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the nullable boolean property and column name.</returns>
    public static ColumnMap<T> For(Func<T, bool?> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(bool),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for a byte-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the byte value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the byte property and column name.</returns>
    public static ColumnMap<T> For(Func<T, byte> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(byte),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for a nullable byte-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the nullable byte value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the nullable byte property and column name.</returns>
    public static ColumnMap<T> For(Func<T, byte?> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(byte),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for a Logix element property with a 16-bit integer (short) data type mapped to the specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the 16-bit integer value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the 16-bit integer property and column name.</returns>
    public static ColumnMap<T> For(Func<T, short> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(short),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for an integer-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the integer value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the integer property and column name.</returns>
    public static ColumnMap<T> For(Func<T, int> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(int),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for an integer-based property of an element with a specified database column name.
    /// </summary>
    /// <param name="getter">A function that retrieves the nullable integer value from the element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{T}"/> configured for the integer property and column name.</returns>
    public static ColumnMap<T> For(Func<T, int?> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(int),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for a float-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the float value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the float property and column name.</returns>
    public static ColumnMap<T> For(Func<T, float> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(float),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for a float-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the float value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the float property and column name.</returns>
    public static ColumnMap<T> For(Func<T, float?> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(float),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for a double-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the double value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{T}"/> configured for the double property and column name.</returns>
    public static ColumnMap<T> For(Func<T, double> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(float),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for a DateTime-based property of a Logix element with a specified database column name.
    /// </summary>
    /// <typeparam name="T">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
    /// <param name="getter">A function that retrieves the DateTime value from the Logix element to be mapped to the database column.</param>
    /// <param name="name">The name of the database column to map the property to.</param>
    /// <param name="hashable">A value indicating whether this column should be included in hash calculations. Defaults to true.</param>
    /// <returns>A new instance of <see cref="ColumnMap{TElement}"/> configured for the DateTime property and column name.</returns>
    public static ColumnMap<T> For(Func<T, DateTime?> getter, string name, bool hashable = true)
    {
        return new ColumnMap<T>
        {
            Name = name,
            Type = typeof(DateTime),
            Getter = x => getter(x),
            Hashable = hashable
        };
    }

    /// <summary>
    /// Creates a new column map for storing a record hash value, specifically configured to map
    /// the computed hash value of a Logix element to a database column named "record_hash".
    /// </summary>
    /// <param name="map">The table map containing configuration and logic for hash computation of the Logix element.</param>
    /// <returns>A new instance of <see cref="ColumnMap{T}"/> configured for mapping the record's computed hash to the "record_hash" database column.</returns>
    public static ColumnMap<T> RecordHash(TableMap<T> map)
    {
        return new ColumnMap<T>
        {
            Name = "record_hash",
            Type = typeof(string),
            Getter = map.ComputeHash,
            Hashable = false
        };
    }
}