using AntiFraud.System.Application.Repositories;
using AntiFraud.System.Infrastructure.Context;
using AntiFraud.System.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AntiFraud.System.Infrastructure.DependencyInjections
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Registamos a mesma classe para as duas interfaces
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITransactionQueryRepository, TransactionRepository>();

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