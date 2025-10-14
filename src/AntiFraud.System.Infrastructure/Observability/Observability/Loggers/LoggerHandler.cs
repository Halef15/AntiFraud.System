using Microsoft.Extensions.Logging;

namespace AntiFraud.System.Infrastructure.Observability.Loggers
{
    /// <summary>
    /// Classe base abstrata que fornece funcionalidades de logging para manipuladores de comando.
    /// </summary>
    /// <typeparam name="T">O tipo associado ao logger.</typeparam>
    public abstract class LoggerHandler<T>
    {
        #region Protected Properties
        /// <summary>
        /// O logger utilizado para registrar informações relacionadas ao manipulador de comando.
        /// </summary>
        protected readonly ILogger<T> _logger;

        /// <summary>
        /// A fábrica de loggers utilizada para criar instâncias de logger.
        /// </summary>
        protected readonly ILoggerFactory _loggerFactory;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor que inicializa a fábrica de loggers e cria uma instância de logger para o tipo especificado.
        /// </summary>
        /// <param name="loggerFactory">A fábrica de loggers utilizada para criar instâncias de logger.</param>
        /// <exception cref="ArgumentNullException">Lançada se a fábrica de loggers for nula.</exception>
        protected LoggerHandler(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<T>();
        }
        #endregion
    }
}