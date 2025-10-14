using Microsoft.Extensions.Logging;

namespace AntiFraud.System.Infrastructure.Observability.Loggers
{
    /// <summary>
    /// Classe que define os eventos de log da aplicação.
    /// </summary>
    public static class AppLogEvents
    {
        #region Public Methods/Operators
        /// <summary>
        /// Cria um evento de log para a operação de criação.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Create(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(1000, nameof(Create)) : new(1000, $"{nameof(Create)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para a operação de leitura.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Read(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(1001, nameof(Read)) : new(1001, $"{nameof(Read)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para a operação de atualização.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Update(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(1002, nameof(Update)) : new(1002, $"{nameof(Update)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para a operação de exclusão.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Delete(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(1003, nameof(Delete)) : new(1003, $"{nameof(Delete)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para detalhes.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Details(string eventSource)
            => !string.IsNullOrWhiteSpace(eventSource) ? new(2000, eventSource) : throw new ArgumentNullException(nameof(eventSource));

        /// <summary>
        /// Cria um evento de log para erros.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Error(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(2001, nameof(Error)) : new(2001, $"{nameof(Error)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para exceções.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Exception(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(2002, nameof(Exception)) : new(2002, $"{nameof(Exception)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para comandos.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Command(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(3000, nameof(Command)) : new(3000, $"{nameof(Command)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para eventos de domínio.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId DomainEvent(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(3001, nameof(DomainEvent)) : new(3001, $"{nameof(DomainEvent)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para eventos.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Event(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(3002, nameof(Event)) : new(3002, $"{nameof(Event)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para notificações.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId Notification(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(3003, nameof(Notification)) : new(3003, $"{nameof(Notification)}-{eventSource}");


        /// <summary>
        /// Cria um evento de log para leituras não encontradas.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId ReadNotFound(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(4000, nameof(ReadNotFound)) : new(4000, $"{nameof(ReadNotFound)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para atualizações não encontradas.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId UpdateNotFound(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(4001, nameof(UpdateNotFound)) : new(4001, $"{nameof(UpdateNotFound)}-{eventSource}");

        /// <summary>
        /// Cria um evento de log para exclusões não encontradas.
        /// </summary>
        /// <param name="eventSource">A fonte do evento.</param>
        /// <returns>O identificador do evento de log.</returns>
        public static EventId DeleteNotFound(string? eventSource = default)
            => string.IsNullOrWhiteSpace(eventSource) ? new(4002, nameof(DeleteNotFound)) : new(4002, $"{nameof(DeleteNotFound)}-{eventSource}");

        /// <summary>
        /// Identificadores de eventos relacionados a processos.
        /// </summary>
        public struct ProcessEvents
        {
            #region Public Methods/Operators
            /// <summary>
            /// Evento de início do processo.
            /// </summary>
            public static EventId Start = new(6000, "Start");

            /// <summary>
            /// Evento de parada do processo.
            /// </summary>
            public static EventId Stop = new(6001, "Stop");

            /// <summary>
            /// Evento de reinício do processo.
            /// </summary>
            public static EventId Restart = new(6002, "Restart");

            /// <summary>
            /// Evento relacionado ao cronograma do processo.
            /// </summary>
            public static EventId Cron = new(6003, "Cron");
            #endregion
        }
        #endregion
    }
}