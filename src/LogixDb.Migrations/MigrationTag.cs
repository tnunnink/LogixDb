namespace LogixDb.Migrations;

/// <summary>
/// 
/// </summary>
public static class MigrationTag
{
    /// <summary>
    /// Marks migrations that are always applied, regardless of user configuration.
    /// Required migrations typically include core schema tables and metadata tables.
    /// </summary>
    public const string Required = nameof(Required);
    
    /// <summary>
    /// 
    /// </summary>
    public const string TagTable = nameof(TagTable);
    
    /// <summary>
    /// 
    /// </summary>
    public const string DataTypeTable = nameof(TagTable);
}