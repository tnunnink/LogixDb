using LogixDb.Data;
using Microsoft.Data.SqlClient;

namespace LogixDb.Service.Common;

/// <summary>
/// Represents the configuration settings used by the LogixDb services for managing database connections,
/// file drop locations, conflict resolution behavior, and FTAC monitoring features.
/// </summary>
/// <remarks>
/// This class provides immutable properties for configuring various aspects of the LogixDb service.
/// These properties can be initialized during application setup to customize the behavior of the system.
/// </remarks>
public class LogixConfig
{
    /// <summary>
    /// Gets the connection string used to establish a connection to the LogixDb database.
    /// </summary>
    /// <remarks>
    /// This property determines how the application connects to the database, including the server address,
    /// authentication, and database selection. It is initialized during configuration and remains immutable.
    /// </remarks>
    public string DbConnection { get; init; } = "logix@localhost";

    /// <summary>
    /// Gets the file system path used as the designated drop location for uploading files
    /// to the LogixDb service.
    /// </summary>
    /// <remarks>
    /// This property defines the directory where files are temporarily stored before being processed
    /// by the system. It must be accessible by the application and preconfigured to ensure successful
    /// file uploads. The drop path is immutable once initialized in configuration settings.
    /// </remarks>
    public string DropPath { get; init; } = @"C:\Program Data\LogixDb";

    /// <summary>
    /// Gets the conflict resolution behavior to apply when handling database snapshots in the LogixDb service.
    /// </summary>
    /// <remarks>
    /// This property determines how the system should handle scenarios where new database snapshots
    /// conflict with existing snapshots. The available options, defined in the <see cref="SnapshotAction"/> enumeration,
    /// include appending the snapshot, replacing the latest snapshot, or replacing all existing snapshots.
    /// </remarks>
    public SnapshotAction OnConflict { get; init; } = SnapshotAction.ReplaceLatest;

    /// <summary>
    /// Indicates whether FTAC (Factory Talk Asset Centre) monitoring service is enabled.
    /// By default, this is disabled. Users will need to opt into this feature.
    /// </summary>
    /// <remarks>
    /// When enabled, this service will monitor for new versions of ACD files. When new versions are created, the service
    /// will download and ingest them into the configured LogixDb. This service assumes the AssetCentre database
    /// is installed locally alongside the Windows service.
    /// </remarks>
    public bool FtacMonitor { get; init; }

    /// <summary>
    /// Overrides the connection string of the FTAC database.
    /// </summary>
    /// <remarks>
    /// This is mainly for testing or overriding the default connection settings. The typical setup will assume this
    /// service is installed locally and runs on the built-in virtual network account. Users are expected to grant
    /// read permissions in the target FTAC database for the service account.
    /// </remarks>
    public string? FtacConnection { get; init; }

    /// <summary>
    /// Gets or sets the collection of FTAC filter configurations used to refine FTAC monitoring behavior.
    /// </summary>
    /// <remarks>
    /// This property contains a set of filter criteria that are applied during FTAC (File Transfer Authentication Code) monitoring.
    /// These filters help determine which files or events are processed based on specified conditions or patterns.
    /// </remarks>
    public string[] FtacFilters { get; init; } = [];

    /// <summary>
    /// Constructs and retrieves the connection string used for FTAC monitoring functionality.
    /// </summary>
    /// <remarks>
    /// If a custom FTAC connection string is provided via the <c>FtacConnection</c> property,
    /// it will be returned as the connection string. Otherwise, a default connection string
    /// for a local AssetCentre database with integrated security is generated and returned.
    /// </remarks>
    /// <returns>
    /// A string representing the FTAC connection string, either as provided in the <c>FtacConnection</c>
    /// property or as a default constructed connection string.
    /// </returns>
    public string GetFtacConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(FtacConnection))
            return FtacConnection;

        return new SqlConnectionStringBuilder
        {
            DataSource = "localhost",
            InitialCatalog = "AssetCentre",
            IntegratedSecurity = true,
            TrustServerCertificate = true
        }.ConnectionString;
    }
}