namespace LogixDb.Data;

/// <summary>
/// Represents a collection of predefined SQL script names used for database operations.
/// </summary>
public enum ScriptName
{
    DeleteTarget,
    DeleteVersion,
    DeleteVersionsBeforeDate,
    DeleteVersionsByNumber,
    GetTargetByLatest,
    GetTargetByVersion,
    ListTargets,
    PostImport,
    PostInfo,
    PostLog,
    PostTarget,
    PostVersion,
    UpdateImport
}