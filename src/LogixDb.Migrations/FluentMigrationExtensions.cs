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
    public static ICreateTableColumnOptionOrWithColumnSyntax WithPrimaryGuid(
        this ICreateTableWithColumnOrSchemaOrDescriptionSyntax syntax,
        string name
    )
    {
        return syntax.WithColumn(name).AsGuid().NotNullable().PrimaryKey();
    }

    extension(ICreateTableColumnOptionOrWithColumnSyntax syntax)
    {
        /// <summary>
        /// Configures the current table with a foreign key to the snapshot table using the snapshot_id column as the
        /// relation column. This column is a non-nullable integer with cascade delete or update options configured. 
        /// </summary>
        /// <returns>Returns the fluent migration builder after adding the snapshot foreign key column.</returns>
        public ICreateTableColumnOptionOrWithColumnSyntax WithSnapshotRelation(bool nullable = false)
        {
            syntax = syntax.WithColumn("snapshot_id").AsInt32();
            syntax = nullable ? syntax.Nullable() : syntax.NotNullable();
            syntax = syntax.ForeignKey("snapshot_instance", "snapshot_id").OnDeleteOrUpdate(Rule.Cascade);
            return syntax;
        }

        /// <summary>
        /// Adds a foreign key column to the table with the specified name and links it to the referenced table.
        /// The column is configured as a non-nullable GUID with cascade delete or update behavior.
        /// </summary>
        /// <param name="columnName">The name of the foreign key column to be added to the table.</param>
        /// <param name="primaryTable">The name of the table that contains the primary key being referenced.</param>
        /// <param name="primaryColumn">The name of the primary key column in the referenced table. If null, uses the same name as columnName.</param>
        /// <param name="nullable">Indicates whether the foreign key column should allow null values. If true, the column is nullable; otherwise, it is not nullable.</param>
        /// <returns>Returns the fluent migration builder after adding and configuring the foreign key column.</returns>
        public ICreateTableColumnOptionOrWithColumnSyntax WithParentRelation(
            string columnName,
            string primaryTable,
            string? primaryColumn = null,
            bool nullable = false
        )
        {
            syntax = syntax.WithColumn(columnName).AsGuid();
            syntax = nullable ? syntax.Nullable() : syntax.NotNullable();
            syntax = syntax.ForeignKey(primaryTable, primaryColumn ?? columnName);
            return syntax;
        }
    }
}