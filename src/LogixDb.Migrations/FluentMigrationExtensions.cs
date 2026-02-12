using System.Data;
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
    public static ICreateTableColumnOptionOrWithColumnSyntax WithPrimaryId(
        this ICreateTableWithColumnOrSchemaOrDescriptionSyntax syntax,
        string name
    )
    {
        return syntax.WithColumn(name)
            .AsInt32()
            .NotNullable()
            .PrimaryKey()
            .Identity();
    }

    /// <summary>
    /// Adds a foreign key column to the table with cascade rules for delete and update operations.
    /// </summary>
    /// <param name="syntax">The fluent migration builder used to define the table schema.</param>
    /// <param name="name">The name of the column to be added as a foreign key.</param>
    /// <param name="foreignTable">The name of the foreign table to which the foreign key points.</param>
    /// <param name="foreignColumn">The name of the column in the foreign table.
    /// If null, the column name will be the same as the foreign key column.</param>
    /// <returns>Returns the fluent migration builder after adding the foreign key column.</returns>
    public static ICreateTableColumnOptionOrWithColumnSyntax WithCascadeForeignKey(
        this ICreateTableColumnOptionOrWithColumnSyntax syntax,
        string name,
        string foreignTable,
        string? foreignColumn = null
    )
    {
        return syntax.WithColumn(name)
            .AsInt32()
            .NotNullable()
            .ForeignKey(foreignTable, foreignColumn ?? name)
            .OnDeleteOrUpdate(Rule.Cascade);
    }
}