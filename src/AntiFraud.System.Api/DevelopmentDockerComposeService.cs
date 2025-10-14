using AntiFraud.System.Application.Utilities;
using Ductus.FluentDocker.Services;

namespace AntiFraud.System.Api
{
    /// <summary>
    /// Serviço hospedado para gerenciar containers Docker Compose em ambiente de desenvolvimento.
    /// </summary>
    public class DevelopmentDockerComposeService : IHostedService
    {
        #region Propriedades Públicas
        /// <summary>
        /// Indica se o Docker Compose está em execução no ambiente de desenvolvimento.
        /// </summary>
        public static bool IsDockerComposeRunning { get; private set; } = false;
        #endregion

        #region Variáveis
        /// <summary>
        /// Serviço composto que representa os contêineres Docker gerenciados.
        /// </summary>
        private ICompositeService? _compositeService;

        /// <summary>
        /// Ambiente de hospedagem da aplicação.
        /// </summary>
        private readonly IHostEnvironment _environment;

        /// <summary>
        /// Interface que permite registrar ações que ocorrem durante o ciclo de vida da aplicação.
        /// </summary>
        private readonly IHostApplicationLifetime _lifetime;

        /// <summary>
        /// Logger para registrar informações sobre a execução do serviço.
        /// </summary>
        private readonly ILogger<DevelopmentDockerComposeService> _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="DevelopmentDockerComposeService"/>.
        /// </summary>
        /// <param name="environment">O ambiente de hospedagem da aplicação.</param>
        /// <param name="lifetime">Gerenciador de ciclo de vida da aplicação.</param>
        /// <param name="logger">Instância de <see cref="ILogger"/> para registrar eventos.</param>
        public DevelopmentDockerComposeService(
            IHostEnvironment environment,
            IHostApplicationLifetime lifetime,
            ILogger<DevelopmentDockerComposeService> logger)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _compositeService = null;
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_environment.IsDevelopment())
                {
                    _logger.LogInformation("[Starting: DockerCompose]-[{Environment}] - [{Application}]", _environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName);

                    // Configurar docker-compose
                    var currentDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "docker"));
                    var serviceName = "antifraud-system-dev";

                    // Inicializar docker-compose
                    _compositeService = new Ductus.FluentDocker.Builders.Builder()
                        .UseContainer()
                        .UseCompose()
                        .ServiceName(serviceName)
                        .FromFile(Path.Combine(currentDirectory, "docker-compose.yaml"))
                        .FromFile(Path.Combine(currentDirectory, "docker-compose-otel.yaml"))
                        .FromFile(Path.Combine(currentDirectory, "docker-compose-aspire-dashboard.yaml"))
                        .FromFile(Path.Combine(currentDirectory, "docker-compose-prometheus.yaml"))
                        .FromFile(Path.Combine(currentDirectory, "docker-compose-rabbitmq.yaml"))
                        .RemoveOrphans()
                        .Build()
                        .Start();

                    _logger.LogInformation("[Started: DockerCompose]-[{Environment}] - [{Application}]", _environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName);
                    _lifetime.ApplicationStopping.Register(OnShutdown);

                    IsDockerComposeRunning = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error: DockerCompose]-[{Environment}] - [{Application}] Erro: {Error}", _environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName, ex.Message);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Métodos/Operadores Privados
        /// <summary>
        /// Manipulador de encerramento da aplicação, responsável por interromper os contêineres Docker.
        /// </summary>
        private void OnShutdown()
        {
            try
            {
                if (_compositeService.IsNullOrEmpty() || _compositeService.State != ServiceRunningState.Running)
                    return;

                _logger.LogInformation("[Stopping: DockerCompose]-[{Environment}] - [{Application}]", _environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName);
                _compositeService.Dispose();
                _logger.LogInformation("[Stopped: DockerCompose]-[{Environment}] - [{Application}]", _environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Error: DockerCompose]-[{Environment}] - [{Application}] Erro: {Error}", _environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName, ex.Message);
            }
        }
        #endregion
    }
}