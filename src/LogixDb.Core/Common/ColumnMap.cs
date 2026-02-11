using System.Linq.Expressions;
using L5Sharp.Core;

namespace LogixDb.Core.Common;

/// <summary>
/// Represents a mapping configuration between a Logix element property and a database column.
/// This record defines how to extract data from an element of type <typeparamref name="TElement"/>
/// and map it to a specific database column with the appropriate type and constraints.
/// </summary>
/// <typeparam name="TElement">The type of Logix element being mapped, which must implement <see cref="ILogixElement"/>.</typeparam>
public sealed record ColumnMap<TElement> where TElement : ILogixElement
{
    /// <summary>
    /// Gets or sets the name of the database column to which a property of <typeparamref name="TElement"/> is mapped.
    /// This represents the column's identifier in the database schema and serves as a key for data mapping.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the database column type to which a property of <typeparamref name="TElement"/> is mapped.
    /// This defines the data type representation for the column in the database schema, used for type-specific operations.
    /// </summary>
    public required ColumnType Type { get; init; }

    /// <summary>
    /// Gets the function used to extract the column value from an instance of <typeparamref name="TElement"/>.
    /// This function is invoked during data import operations to retrieve the value to be stored in the database.
    /// </summary>
    public required Func<TElement, object?> Getter { get; init; }

    /// <summary>
    /// Creates a new <see cref="ColumnMap{TElement}"/> instance for the specified property or field of a
    /// Logix element and assigns it a database column name.
    /// </summary>
    /// <param name="getter">An expression that identifies the property or field of the <typeparamref name="TElement"/>
    /// to be mapped to the database column. This expression must specify how to retrieve the property's value.</param>
    /// <param name="name">The name of the database column that the specified property or field is mapped to.</param>
    /// <returns>A configured <see cref="ColumnMap{TElement}"/> instance representing the mapping of the specified
    /// property or field to the database column.</returns>
    public static ColumnMap<TElement> For(Expression<Func<TElement, object>> getter, string name)
    {
        var returnType = getter.Body.Type;

        if (getter.Body.Type == typeof(object) && getter.Body is UnaryExpression unary)
        {
            returnType = unary.Operand.Type;
        }

        var dbType = GetDbType(returnType);

        return new ColumnMap<TElement>
        {
            Name = name,
            Type = dbType,
            Getter = getter.Compile()
        };
    }

    /// <summary>
    /// Determines the appropriate database column type corresponding to the specified .NET type.
    /// </summary>
    /// <param name="type">The .NET type for which the corresponding database column type is required.</param>
    /// <returns>A <see cref="ColumnType"/> value that represents the database mapping for the specified .NET type.</returns>
    private static ColumnType GetDbType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType switch
        {
            _ when underlyingType == typeof(bool) => ColumnType.Boolean,
            _ when underlyingType == typeof(short) => ColumnType.Int16,
            _ when underlyingType == typeof(int) => ColumnType.Int32,
            _ when underlyingType == typeof(string) => ColumnType.Text,
            _ when underlyingType == typeof(DateTime) => ColumnType.DateTime,
            _ when underlyingType == typeof(byte[]) => ColumnType.Blob,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type,
                $"Unsupported type '{type.FullName}' for database column mapping.")
        };
    }
}