using FluentMigrator.Builders.Create.Table;

namespace LogixDb.Migrations;

/// <summary>
/// Provides extension methods to simplify FluentMigrator table creation and manipulation.
/// </summary>
public static class FluentMigrationExtensions
{
    /// <summary>
    /// Adds a primary key column to the table with auto-increment functionality.
    /// </summary>
    /// <param name="syntax">The fluent migration builder used to define the table schema.</param>
    /// <param name="name">The name of the column to be added as the primary key.</param>
    /// <returns>Returns the fluent migration builder after adding the primary key column.</returns>
    public static ICreateTableColumnOptionOrWithColumnSyntax WithPrimaryKey(
        this ICreateTableWithColumnOrSchemaOrDescriptionSyntax syntax,
        string name
    )
    {
        return syntax.WithColumn(name).AsGuid().NotNullable().PrimaryKey();
    }

    /// <summary>
    /// Adds a column to the table that establishes a foreign key relationship with another table.
    /// The column type is determined based on the specified generic type parameter.
    /// Supported types are <see cref="Guid"/> and <see cref="int"/>.
    /// </summary>
    /// <param name="syntax">The fluent migration builder used to define the table schema.</param>
    /// <param name="columnName">The name of the column to be added as the foreign key.</param>
    /// <param name="primaryTable">The name of the primary table that the foreign key references.</param>
    /// <param name="primaryColumn">The name of the primary column in the referenced table.
    /// Defaults to the name of the foreign key column if not specified.</param>
    /// <typeparam name="T">The data type of the foreign key column, which must be either <see cref="Guid"/> or <see cref="int"/>.</typeparam>
    /// <returns>Returns the fluent migration builder after adding the column with the foreign key relationship.</returns>
    /// <exception cref="NotSupportedException">Thrown when the specified generic type parameter is not <see cref="Guid"/> or <see cref="int"/>.</exception>
    public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithRelation<T>(
        this ICreateTableColumnOptionOrWithColumnSyntax syntax,
        string columnName,
        string primaryTable,
        string? primaryColumn = null
    ) where T : struct
    {
        if (typeof(T) == typeof(Guid))
        {
            return syntax
                .WithColumn(columnName)
                .AsGuid()
                .ForeignKey(primaryTable, primaryColumn ?? columnName);
        }

        if (typeof(T) == typeof(int))
        {
            return syntax
                .WithColumn(columnName)
                .AsInt32()
                .ForeignKey(primaryTable, primaryColumn ?? columnName);
        }

        throw new NotSupportedException(
            $"Type {typeof(T).Name} is not supported for foreign key relations. Only Guid and int are supported.");
    }
}