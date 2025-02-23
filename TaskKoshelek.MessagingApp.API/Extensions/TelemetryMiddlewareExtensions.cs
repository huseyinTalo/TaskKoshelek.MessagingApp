using TaskKoshelek.MessagingApp.API.Middleware;

namespace TaskKoshelek.MessagingApp.API.Extensions
{
    public static class TelemetryMiddlewareExtensions
    {
        public static IApplicationBuilder UseTelemetryMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TelemetryMiddleware>();
        }
    }
}
