using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Findly.Migrations;

/// <summary>
/// Application entry point that orchestrates the migration process.
/// </summary>
public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            // Load configuration
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var configuration = BuildConfiguration(args, environment);

            // Configure logging
            SerilogConfiguration.Configure(configuration, environment);

            // Create logger
            using var loggerFactory = new LoggerFactory().AddSerilog();
            var logger = loggerFactory.CreateLogger<MigrationRunner>();

            // Validate configuration
            var connectionString = configuration.GetConnectionString(MigrationConfig.CONNECTION_STRING_NAME);
            if (string.IsNullOrEmpty(connectionString))
            {
                logger.LogError("Connection string '{ConnectionString}' not found in configuration.",
                    MigrationConfig.CONNECTION_STRING_NAME);
                return -1;
            }

            // Execute migrations
            var runner = new MigrationRunner(logger, connectionString);
            var success = runner.Execute();

            if (success)
            {
                logger.LogInformation("Migration completed successfully!");
                return 0;
            }
            else
            {
                logger.LogError("Migration failed. Check the logs for details.");
                return -1;
            }
        }
        catch (Exception ex)
        {
            // Try to log with Serilog if available, otherwise use console
            if (Log.Logger != null)
            {
                Log.Error(ex, "Unhandled exception during migration");
            }
            else
            {
                Console.Error.WriteLine($"Unhandled exception during migration: {ex}");
            }
            return -1;
        }
        finally
        {
            // Ensure all logs are flushed
            SerilogConfiguration.CloseAndFlush();
        }
    }

    private static IConfiguration BuildConfiguration(string[] args, string environment)
    {
        return new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
    }
}
