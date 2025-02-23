using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace TaskKoshelek.MessagingApp.API.Middleware
{
    public class TelemetryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly string _telemetryDbConnectionString;

        public TelemetryMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            _telemetryDbConnectionString = _configuration.GetConnectionString("TelemetryDatabase");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Don't log requests to health check endpoints or static files
            if (context.Request.Path.StartsWithSegments("/health") ||
                context.Request.Path.StartsWithSegments("/static"))
            {
                await _next(context);
                return;
            }

            var correlationId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();

            // Start timing
            var stopwatch = Stopwatch.StartNew();
            var startTime = DateTime.UtcNow;

            // Capture the original response body stream
            var originalBodyStream = context.Response.Body;
            var responseBodyContent = string.Empty;

            // Capture request body
            string requestBodyContent = await GetRequestBodyAsync(context.Request);

            try
            {
                // Create a new memory stream for the response
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // Continue down the middleware pipeline
                await _next(context);

                // Read the response body
                responseBody.Seek(0, SeekOrigin.Begin);
                responseBodyContent = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);

                // Copy the response to the original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
            finally
            {
                // Restore the original response body stream
                context.Response.Body = originalBodyStream;

                // Calculate duration
                stopwatch.Stop();
                var endTime = DateTime.UtcNow;
                var durationMs = (int)stopwatch.ElapsedMilliseconds;

                // Log request details to telemetry database
                await LogRequestToTelemetryDatabaseAsync(
                    context.Request.Path,
                    context.Request.Method,
                    context.Response.StatusCode,
                    startTime,
                    endTime,
                    durationMs,
                    GetClientIpAddress(context),
                    TruncateIfTooLong(requestBodyContent, 8000),  // Limit to 8000 chars to fit in NVARCHAR(MAX)
                    TruncateIfTooLong(responseBodyContent, 8000),
                    correlationId);
            }
        }

        private async Task<string> GetRequestBodyAsync(HttpRequest request)
        {
            // Only attempt to read the body if it's a POST, PUT, or PATCH request
            if (request.Method == "GET" || request.Method == "DELETE" ||
                request.Method == "HEAD" || request.Method == "OPTIONS")
            {
                return string.Empty;
            }

            // Ensure the request body can be read multiple times
            request.EnableBuffering();

            using var reader = new StreamReader(
                request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();

            // Reset the position to the beginning to allow reading again
            request.Body.Position = 0;

            return body;
        }

        private string GetClientIpAddress(HttpContext context)
        {
            // Try X-Forwarded-For header first
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            // Fallback to connection remote IP
            if (context.Connection.RemoteIpAddress != null)
            {
                var ip = context.Connection.RemoteIpAddress.ToString();

                // Convert IPv6 loopback to more readable format
                if (ip == "::1")
                {
                    return "127.0.0.1";
                }

                return ip;
            }

            return "unknown";
        }

        private string TruncateIfTooLong(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            {
                return text;
            }

            return text.Substring(0, maxLength) + "... [truncated]";
        }

        private async Task LogRequestToTelemetryDatabaseAsync(
            string path,
            string method,
            int statusCode,
            DateTime startTime,
            DateTime endTime,
            int durationMs,
            string ipAddress,
            string requestBody,
            string responseBody,
            string correlationId)
        {
            try
            {
                using (var connection = new SqlConnection(_telemetryDbConnectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new SqlCommand(
                        @"INSERT INTO RequestLogs 
                            (Path, Method, StatusCode, StartTime, EndTime, DurationMs, 
                             IpAddress, RequestBody, ResponseBody, CorrelationId) 
                          VALUES 
                            (@path, @method, @statusCode, @startTime, @endTime, @durationMs,
                              @ipAddress, @requestBody, @responseBody, @correlationId)",
                        connection))
                    {
                        cmd.Parameters.Add("@path", SqlDbType.NVarChar, 500).Value = path;
                        cmd.Parameters.Add("@method", SqlDbType.NVarChar, 10).Value = method;
                        cmd.Parameters.Add("@statusCode", SqlDbType.Int).Value = statusCode;
                        cmd.Parameters.Add("@startTime", SqlDbType.DateTime).Value = startTime;
                        cmd.Parameters.Add("@endTime", SqlDbType.DateTime).Value = endTime;
                        cmd.Parameters.Add("@durationMs", SqlDbType.Int).Value = durationMs;
                        cmd.Parameters.Add("@ipAddress", SqlDbType.NVarChar, 50).Value = ipAddress;
                        cmd.Parameters.Add("@requestBody", SqlDbType.NVarChar).Value =
                            string.IsNullOrEmpty(requestBody) ? (object)DBNull.Value : requestBody;
                        cmd.Parameters.Add("@responseBody", SqlDbType.NVarChar).Value =
                            string.IsNullOrEmpty(responseBody) ? (object)DBNull.Value : responseBody;
                        cmd.Parameters.Add("@correlationId", SqlDbType.NVarChar, 100).Value = correlationId;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Also log performance metrics for this request
                    using (var cmdMetrics = new SqlCommand(
                        @"INSERT INTO PerformanceMetrics 
                            (MetricName, Value, Unit, Timestamp, Context) 
                          VALUES 
                            (@metricName, @value, @unit, @timestamp, @context)",
                        connection))
                    {
                        cmdMetrics.Parameters.Add("@metricName", SqlDbType.NVarChar, 100).Value = "HttpRequestDuration";
                        cmdMetrics.Parameters.Add("@value", SqlDbType.Float).Value = durationMs;
                        cmdMetrics.Parameters.Add("@unit", SqlDbType.NVarChar, 50).Value = "ms";
                        cmdMetrics.Parameters.Add("@timestamp", SqlDbType.DateTime).Value = endTime;
                        cmdMetrics.Parameters.Add("@context", SqlDbType.NVarChar, 500).Value =
                            $"Path={path}, Method={method}, StatusCode={statusCode}, CorrelationId={correlationId}";

                        await cmdMetrics.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log to console since we can't log to the database
                Console.WriteLine($"Failed to log request to telemetry database: {ex.Message}");
            }
        }
    }
}
