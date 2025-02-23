using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskKoshelek.MessagingApp.Core.Interfaces;
using TaskKoshelek.MessagingApp.Core.Utilities;
using TaskKoshelek.MessagingApp.DAL.Concrete;

namespace TaskKoshelek.MessagingApp.DAL.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["DB_CONNECTION_STRING"];

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = configuration.GetSection("DB_CONNECTION_STRING").ToString();
            }

            services.Configure<DatabaseOptions>(options =>
            {
                options.ConnectionString = connectionString;
            });

            if (string.IsNullOrEmpty(configuration.GetConnectionString("TelemetryDatabase")))
            {
                Console.WriteLine("Warning: TelemetryDatabase connection string is missing in configuration!");
            }

            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<ILogToDatabaseRepository, LogToDatabaseRepository>();
            return services;
        }
    }
}
