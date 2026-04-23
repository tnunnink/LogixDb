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
    /// Adds a foreign key relation to a column, linking it to a primary key in another table.
    /// </summary>
    /// <param name="syntax">The fluent migration builder used to define the table schema.</param>
    /// <param name="columnName">The name of the column to which the foreign key relation is added.</param>
    /// <param name="primaryTable">The name of the primary table containing the referenced key.</param>
    /// <param name="primaryColumn">The name of the primary key column in the primary table. Defaults to the value of <paramref name="columnName"/> if not specified.</param>
    /// <returns>Returns the fluent migration builder after adding the foreign key relation.</returns>
    public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithRelation(
        this ICreateTableColumnOptionOrWithColumnSyntax syntax,
        string columnName,
        string primaryTable,
        string? primaryColumn = null
    )
    {
        return syntax
            .WithColumn(columnName)
            .AsGuid()
            .ForeignKey(primaryTable, primaryColumn ?? columnName);
    }
}