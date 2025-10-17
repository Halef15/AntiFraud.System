using AntiFraud.System.Application.Repositories;
using AntiFraud.System.Application.Utilities;
using AntiFraud.System.Infrastructure.Context;
using AntiFraud.System.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AntiFraud.System.Infrastructure.DependencyInjections
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMassTransitConsumerInfrastructure(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            // RabbitMQ
            return services.AddMassTransit(registration =>
            {
                registration.AddConsumers(Assembly.GetEntryAssembly());

                registration.SetKebabCaseEndpointNameFormatter();
                registration.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureNewtonsoftJsonSerializer(options => JsonSettings.RestSerializerSettings);
                    cfg.ConfigureNewtonsoftJsonDeserializer(options => JsonSettings.RestSerializerSettings);
                    cfg.Host(configuration.GetConnectionString("RabbitMq"));
                    cfg.ConfigureEndpoints(context);
                });
            });
        }

        public static IServiceCollection AddMassTransitPublisherInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // RabbitMQ
            return services.AddMassTransit(registration =>
            {
                registration.SetKebabCaseEndpointNameFormatter();
                registration.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureNewtonsoftJsonSerializer(options => JsonSettings.RestSerializerSettings);
                    cfg.ConfigureNewtonsoftJsonDeserializer(options => JsonSettings.RestSerializerSettings);
                    cfg.Host(configuration.GetConnectionString("RabbitMq"));
                    cfg.ConfigureEndpoints(context);
                });
            });
        }

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Registamos a mesma classe para as duas interfaces
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionQueryRepository, TransactionRepository>();

            services.AddScoped<IBlockedCardRepository, BlockedCardRepository>();

            // UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // DbContext (código restante igual)
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }
    }
}