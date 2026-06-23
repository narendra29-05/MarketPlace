using System.Reflection;
using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Logging;

namespace Findly.Migrations;

/// <summary>
/// Handles database migration execution using DbUp.
/// </summary>
public class MigrationRunner
{
    private readonly ILogger<MigrationRunner> _logger;
    private readonly string _connectionString;

    public MigrationRunner(ILogger<MigrationRunner> logger, string connectionString)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Executes database migrations for the configured schema.
    /// </summary>
    /// <returns>True if migration succeeded, false otherwise.</returns>
    public bool Execute()
    {
        LogHeader();
        return MigrateSchema(MigrationConfig.SCHEMA_NAME);
    }

    private void LogHeader()
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("       {ServiceName} DB Migrator", MigrationConfig.SERVICE_NAME);
        _logger.LogInformation("========================================");
        _logger.LogInformation("Starting {ServiceName} Service Persistence Migration", MigrationConfig.SERVICE_NAME);
        _logger.LogInformation("----------------------------------------");
    }

    private bool MigrateSchema(string schema)
    {
        var scriptsPath = $"Scripts.{schema}";

        _logger.LogInformation("Migrating {Schema} schema...", schema);

        // Skip EnsureDatabase when using pgbouncer since it requires access to postgres database
        // The database should already exist
        // EnsureDatabase.For.PostgresqlDatabase(_connectionString);

        var upgrader = CreateUpgrader(scriptsPath);
        var scriptsToExecute = upgrader.GetScriptsToExecute();

        if (!scriptsToExecute.Any())
        {
            _logger.LogInformation("✓ No pending migrations for {Schema} schema", schema);
            return true;
        }

        _logger.LogInformation("Found {Count} pending migration(s)", scriptsToExecute.Count);
        LogPendingScripts(scriptsToExecute);

        var result = PerformUpgrade(scriptsPath);

        if (result.Successful)
        {
            _logger.LogInformation("✓ Successfully migrated {Schema} schema", schema);
            LogExecutedScripts(result.Scripts);
            return true;
        }
        else
        {
            _logger.LogError(result.Error, "✗ Failed to migrate {Schema} schema", schema);
            return false;
        }
    }

    private UpgradeEngine CreateUpgrader(string scriptsPath)
    {
        return DeployChanges.To
            .SqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly(),
                s => s.StartsWith($"{MigrationConfig.ASSEMBLY_PREFIX}.{scriptsPath}", StringComparison.OrdinalIgnoreCase))
            .WithVariablesDisabled()
            .LogToConsole()
            .Build();
    }

    private DatabaseUpgradeResult PerformUpgrade(string scriptsPath)
    {
        var upgrader = CreateUpgrader(scriptsPath);
        return upgrader.PerformUpgrade();
    }

    private void LogPendingScripts(IEnumerable<SqlScript> scripts)
    {
        _logger.LogInformation("Pending scripts:");
        foreach (var script in scripts)
        {
            _logger.LogInformation("  - {ScriptName} [Pending]", Path.GetFileName(script.Name));
        }
    }

    private void LogExecutedScripts(IEnumerable<SqlScript> scripts)
    {
        if (!scripts.Any())
        {
            return;
        }

        _logger.LogInformation("Executed scripts:");
        foreach (var script in scripts)
        {
            _logger.LogInformation("  - {ScriptName}", Path.GetFileName(script.Name));
        }
    }
}
