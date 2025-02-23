using AutoMapper;
using TaskKoshelek.MessagingApp.API.APIServicves.Abstracts;
using TaskKoshelek.MessagingApp.API.APIServicves.Concretes;
using TaskKoshelek.MessagingApp.API.Profiles.MessageProfiles;

namespace TaskKoshelek.MessagingApp.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAPIServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MessageProfile));
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<ILogDbService, LogDbService>();

            return services;
        }
    }
}