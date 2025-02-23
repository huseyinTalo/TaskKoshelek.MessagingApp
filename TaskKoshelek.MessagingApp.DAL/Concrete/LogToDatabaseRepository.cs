using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskKoshelek.MessagingApp.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace TaskKoshelek.MessagingApp.DAL.Concrete
{
    public class LogToDatabaseRepository : ILogToDatabaseRepository
    {
        private readonly string _telemetryDbConnectionString;
        private readonly Dictionary<string, Stopwatch> _operationTimers = new Dictionary<string, Stopwatch>();

        public LogToDatabaseRepository(IConfiguration configuration)
        {
            _telemetryDbConnectionString = configuration.GetConnectionString("TelemetryDatabase");
        }

        /// <summary>
        /// Starts timing an operation
        /// </summary>
        public void StartOperationTimer(string operationName)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var key = $"{operationName}_{Activity.Current?.Id ?? Guid.NewGuid().ToString()}";

            lock (_operationTimers)
            {
                if (_operationTimers.ContainsKey(key))
                {
                    _operationTimers[key] = stopwatch;
                }
                else
                {
                    _operationTimers.Add(key, stopwatch);
                }
            }
        }

        /// <summary>
        /// Gets elapsed time for an operation in milliseconds
        /// </summary>
        private int? GetOperationElapsedTime(string operationName)
        {
            var key = $"{operationName}_{Activity.Current?.Id ?? Guid.NewGuid().ToString()}";

            lock (_operationTimers)
            {
                if (_operationTimers.TryGetValue(key, out var stopwatch))
                {
                    var elapsedMs = (int)stopwatch.ElapsedMilliseconds;
                    _operationTimers.Remove(key);
                    return elapsedMs;
                }
            }

            return null;
        }

        /// <summary>
        /// Logs operation details to the separate telemetry database
        /// </summary>
        public async Task LogOperationToDatabase(string operation, string status, string details)
        {
            try
            {
                // Get current Activity for correlation ID
                var currentActivity = Activity.Current;
                string correlationId = currentActivity?.TraceId.ToString() ?? Guid.NewGuid().ToString();

                // Get manual execution time from our timers
                int? executionTimeMs = GetOperationElapsedTime(operation);

                // If we don't have a manual timer, try to use Activity.Duration
                if (!executionTimeMs.HasValue && currentActivity?.Duration != null)
                {
                    executionTimeMs = (int)currentActivity.Duration.TotalMilliseconds;
                }

                using (var connection = new SqlConnection(_telemetryDbConnectionString))
                {
                    await connection.OpenAsync();

                    using (var cmd = new SqlCommand(
                        @"INSERT INTO OperationLogs 
                            (Operation, Status, Details, Timestamp, CorrelationId, ExecutionTimeMs) 
                          VALUES 
                            (@operation, @status, @details, @timestamp, @correlationId, @executionTimeMs)",
                        connection))
                    {
                        cmd.Parameters.Add("@operation", SqlDbType.NVarChar, 100).Value = operation;
                        cmd.Parameters.Add("@status", SqlDbType.NVarChar, 50).Value = status;
                        cmd.Parameters.Add("@details", SqlDbType.NVarChar, 4000).Value = details;
                        cmd.Parameters.Add("@timestamp", SqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@correlationId", SqlDbType.NVarChar, 100).Value = correlationId;
                        cmd.Parameters.Add("@executionTimeMs", SqlDbType.Int).Value = executionTimeMs.HasValue ? (object)executionTimeMs.Value : DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Log performance metrics for successful operations
                    if (status == "Success" && executionTimeMs.HasValue)
                    {
                        using (var cmdMetrics = new SqlCommand(
                            @"INSERT INTO PerformanceMetrics 
                                (MetricName, Value, Unit, Timestamp, Context) 
                              VALUES 
                                (@metricName, @value, @unit, @timestamp, @context)",
                            connection))
                        {
                            cmdMetrics.Parameters.Add("@metricName", SqlDbType.NVarChar, 100).Value = $"{operation}ExecutionTime";
                            cmdMetrics.Parameters.Add("@value", SqlDbType.Float).Value = executionTimeMs.Value;
                            cmdMetrics.Parameters.Add("@unit", SqlDbType.NVarChar, 50).Value = "ms";
                            cmdMetrics.Parameters.Add("@timestamp", SqlDbType.DateTime).Value = DateTime.Now;
                            cmdMetrics.Parameters.Add("@context", SqlDbType.NVarChar, 500).Value = $"CorrelationId={correlationId}";

                            await cmdMetrics.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Just log to console as this is the logging mechanism itself
                Console.WriteLine($"Failed to log operation: {ex.Message}");
            }
        }
    }
}