// ReSharper disable InconsistentNaming
namespace LogixDb.Data;

/// <summary>
/// Specifies the type of sender that initiated a database operation or session.
/// Used to track the origin of requests in the LogixDb framework for auditing and logging purposes.
/// </summary>
public enum SourceType
{
    /// <summary>
    /// Represents operations initiated from the Command Line Interface (CLI).
    /// </summary>
    CLI,

    /// <summary>
    /// Represents operations initiated through the Application Programming Interface (API).
    /// </summary>
    API,

    /// <summary>
    /// Represents operations initiated from FactoryTalk AssetCentre (FTAC).
    /// </summary>
    FTAC
}