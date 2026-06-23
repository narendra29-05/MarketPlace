using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

namespace Findly.Migrations;

/// <summary>
/// Handles Serilog configuration and setup.
/// </summary>
public static class SerilogConfiguration
{
    /// <summary>
    /// Configures Serilog with console output and optional OpenTelemetry sink.
    /// </summary>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="environment">Current environment name.</param>
    public static void Configure(IConfiguration configuration, string environment)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithProperty("Service", $"{MigrationConfig.SERVICE_NAME.ToLower()}-migrations-runner")
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");

        // Add OpenTelemetry sink if endpoint is configured
        ConfigureOpenTelemetry(loggerConfig, configuration);

        Log.Logger = loggerConfig.CreateLogger();
    }

    /// <summary>
    /// Ensures all buffered logs are written before application exit.
    /// </summary>
    public static void CloseAndFlush()
    {
        Log.CloseAndFlush();
    }

    private static void ConfigureOpenTelemetry(LoggerConfiguration loggerConfig, IConfiguration configuration)
    {
        var otelEndpoint = configuration["Serilog:OtelEndpoint"];
        if (string.IsNullOrWhiteSpace(otelEndpoint))
        {
            return;
        }

        try
        {
            loggerConfig.WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = otelEndpoint;
                options.Protocol = OtlpProtocol.Grpc;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = $"{MigrationConfig.SERVICE_NAME.ToLower()}-migrations-runner",
                    ["service.version"] = MigrationConfig.SERVICE_VERSION
                };
            });
        }
        catch (Exception ex)
        {
            // If OTEL configuration fails, continue with console logging only
            Console.WriteLine($"Warning: Failed to configure OpenTelemetry sink: {ex.Message}");
        }
    }
}
