namespace Findly.Migrations;

/// <summary>
/// Service-specific configuration for database migrations.
/// Update these values when copying to a new service.
/// </summary>
public static class MigrationConfig
{
    /// <summary>
    /// The name of the service (used in logging and display).
    /// </summary>
    public const string SERVICE_NAME = "Findly";

    /// <summary>
    /// The connection string name in appsettings.json.
    /// </summary>
    public const string CONNECTION_STRING_NAME = "FindlyDB";

    /// <summary>
    /// The database schema name for this service.
    /// </summary>
    public const string SCHEMA_NAME = "fin";

    /// <summary>
    /// The assembly prefix for embedded SQL scripts.
    /// Should match the namespace of this project.
    /// </summary>
    public const string ASSEMBLY_PREFIX = "Findly.Migrations";

    /// <summary>
    /// The service version for telemetry.
    /// </summary>
    public const string SERVICE_VERSION = "1.0.0";
}
