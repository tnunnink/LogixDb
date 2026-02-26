namespace LogixDb.Service.Configuration;

public class FtacConfig
{
    /// <summary>
    /// Indicates whether the feature or functionality is enabled.
    /// </summary>
    /// <remarks>
    /// This property is used to toggle the activation state. By default, its value is determined by the application configuration.
    /// </remarks>
    public bool Enabled { get; init; }

    /// <summary>
    /// Gets the server address or name used for configuration.
    /// </summary>
    /// <remarks>
    /// By default, the server is set to "localhost".
    /// </remarks>
    public string Server { get; init; } = "localhost";

    /// <summary>
    /// Specifies the name of the database to be used by the application.
    /// </summary>
    /// <remarks>
    /// This property defines the target database to establish a connection with. The default value is set to "AssetCentre".
    /// </remarks>
    public string Database { get; init; } = "AssetCentre";

    /// <summary>
    /// Specifies the port number used to establish a connection to the server.
    /// </summary>
    /// <remarks>
    /// This property defines the network port for communication with the server. The default value is typically set to 1433 for SQL Server connections.
    /// </remarks>
    public int Port { get; init; } = 1433;

    /// <summary>
    /// Represents a collection of filters applied to refine or restrict the scope of data processing.
    /// </summary>
    /// <remarks>
    /// This property is typically used to specify criteria for data selection or exclusion. The exact usage will depend on the context in which it is applied.
    /// </remarks>
    public string[] Filters { get; init; } = [];
}