namespace LogixDb.Data;

/// <summary>
/// Flags enum that specifies which component tables should be created during database migration.
/// Multiple options can be combined using bitwise operators.
/// </summary>
[Flags]
public enum ComponentOptions
{
    /// <summary>
    /// No component tables should be created during migration.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// Includes all controller related tables during migration.
    /// </summary>
    Controller = 1,
    
    /// <summary>
    /// Includes all data type related tables during migration.
    /// </summary>
    DataType = 2,
    
    /// <summary>
    /// Includes all Add-On Instruction (AOI) related tables during migration.
    /// </summary>
    Aoi = 4,
    
    /// <summary>
    /// Includes all module related tables during migration.
    /// </summary>
    Module = 8,
    
    /// <summary>
    /// Includes all tag related tables during migration.
    /// </summary>
    Tag = 16,
    
    /// <summary>
    /// Includes all logic related tables during migration.
    /// </summary>
    Logic = 32,
    
    /// <summary>
    /// Include all component tables during migration.
    /// </summary>
    All = Controller | DataType | Aoi | Module | Tag | Logic
}