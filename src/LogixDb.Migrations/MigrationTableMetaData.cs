using FluentMigrator.Runner.VersionTableInfo;

namespace LogixDb.Migrations;

/// <summary>
/// Provides metadata about the migration table used by FluentMigrator.
/// Implements the <see cref="IVersionTableMetaData"/> interface to specify schema details
/// and properties for tracking database migrations in a structured manner.
/// </summary>
[VersionTableMetaData]
public class MigrationTableMetaData : IVersionTableMetaData
{
    public bool OwnsSchema => true;
    public string SchemaName => null!;
    public string TableName => "migration";
    public string ColumnName => "version";
    public string DescriptionColumnName => "description";
    public string UniqueIndexName => "uc_version";
    public string AppliedOnColumnName => "applied_on";
    public bool CreateWithPrimaryKey => false;
}