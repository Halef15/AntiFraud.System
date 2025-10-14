namespace AntiFraud.System.Api.Controllers
{
    /// <summary>
    /// Extensões para configuração de serviços hospedados (Hosted Services) na API.
    /// </summary>
    public static class HostedServiceExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Adiciona serviços de formatação de entrada personalizados ao contêiner de injeção de dependência.
        /// </summary>
        /// <param name="services">A coleção de serviços do sistema atual.</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomHostedService(this IServiceCollection services)
        {
#if DEBUG
            // Serviços de Docker Compose
            services.AddHostedService<DevelopmentDockerComposeService>();
#endif
            return services;
        }
        #endregion
    }
}