namespace LogixDb.Service.Common;

/// <summary>
/// Provides constant file system path values used throughout the LogixDb service.
/// </summary>
public static class Paths
{
    /// <summary>
    /// The root installation directory for the LogixDb service.
    /// </summary>
    public const string ServiceRoot = @"C:\Program Files\LogixDb";
    
    /// <summary>
    /// The directory path where uploaded files are temporarily stored before processing.
    /// </summary>
    public const string Dropzone = @"C:\ProgramData\LogixDb\Dropzone";
}