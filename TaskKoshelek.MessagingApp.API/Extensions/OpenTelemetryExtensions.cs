using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace TaskKoshelek.MessagingApp.API.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Create a resource builder for common properties
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService("TaskKoshelek.MessagingApp")
                .AddEnvironmentVariableDetector()
                .AddTelemetrySdk()
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = configuration["Environment"] ?? "development"
                });

            // Configure tracing
            services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(
                    serviceName: "TaskKoshelek.MessagingApp",
                    serviceVersion: typeof(OpenTelemetryExtensions).Assembly.GetName().Version?.ToString() ?? "unknown"))
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .AddSource("TaskKoshelek.MessagingApp.API") // Our custom ActivitySource
                        .SetResourceBuilder(resourceBuilder)
                        .SetSampler(new AlwaysOnSampler())
                        .AddHttpClientInstrumentation(options =>
                        {
                            options.RecordException = true;
                            options.FilterHttpRequestMessage = (request) =>
                            {
                                // Don't trace health check requests or similar
                                return !request.RequestUri.PathAndQuery.Contains("/health");
                            };
                        })
                        .AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = true;
                            options.Filter = (httpContext) =>
                            {
                                // Filter out health checks or other internal endpoints
                                return !httpContext.Request.Path.StartsWithSegments("/health");
                            };
                            // Capture HTTP request/response headers for better debugging
                            options.EnrichWithHttpRequest = (activity, httpRequest) =>
                            {
                                activity.SetTag("http.request.header.x_forwarded_for", httpRequest.Headers["X-Forwarded-For"].ToString());
                            };
                        })
                        .AddSqlClientInstrumentation(options =>
                        {
                            options.SetDbStatementForText = true;
                            options.RecordException = true;
                        });

                    // Add console exporter for development/debugging
                    tracerProviderBuilder.AddConsoleExporter();

                    // Add OTLP exporter if configured
                    var otlpEndpoint = configuration["OpenTelemetry:OtlpEndpoint"];
                    if (!string.IsNullOrEmpty(otlpEndpoint))
                    {
                        tracerProviderBuilder.AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new Uri(otlpEndpoint);

                            // Optional: Add protocol configuration
                            otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;

                            // Optional: Add headers for authentication if needed
                            if (!string.IsNullOrEmpty(configuration["OpenTelemetry:ApiKey"]))
                            {
                                otlpOptions.Headers = $"api-key={configuration["OpenTelemetry:ApiKey"]}";
                            }
                        });
                    }
                })
                .WithMetrics(metricsProviderBuilder =>
                {
                    metricsProviderBuilder
                        .SetResourceBuilder(resourceBuilder)
                        .AddRuntimeInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation();

                    // Add console exporter for development/debugging
                    metricsProviderBuilder.AddConsoleExporter();

                    // Add OTLP exporter if configured
                    var otlpEndpoint = configuration["OpenTelemetry:OtlpEndpoint"];
                    if (!string.IsNullOrEmpty(otlpEndpoint))
                    {
                        metricsProviderBuilder.AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new Uri(otlpEndpoint);

                            // Optional: Add protocol configuration
                            otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;

                            // Optional: Add headers for authentication if needed
                            if (!string.IsNullOrEmpty(configuration["OpenTelemetry:ApiKey"]))
                            {
                                otlpOptions.Headers = $"api-key={configuration["OpenTelemetry:ApiKey"]}";
                            }
                        });
                    }
                });

            // Register the ActivitySource that we'll use for manual instrumentation
            services.AddSingleton(new ActivitySource("TaskKoshelek.MessagingApp.API"));

            return services;
        }
    }
}
