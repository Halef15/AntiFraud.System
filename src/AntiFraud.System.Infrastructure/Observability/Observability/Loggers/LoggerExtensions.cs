using AntiFraud.System.Application.Utilities;
using AntiFraud.System.Infrastructure.Observability.Settings;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;

namespace AntiFraud.System.Infrastructure.Observability.Loggers
{
    /// <summary>
    /// Métodos de extensão para facilitar a aplicação de logs.
    /// </summary>
    public static class LoggerExtensions
    {
        #region Public Methods/Operators Base
        /// <summary>
        /// Registra um log com escopo enriquecido, associando metadados adicionais ao log, e retorna uma string representando as informações do log gerado.
        /// </summary>
        /// <param name="logger">A instância de <see cref="ILogger"/> usada para registrar o log.</param>
        /// <param name="logLevel">O nível de severidade do log (<see cref="LogLevel"/>).</param>
        /// <param name="message">A mensagem que será registrada no log. Pode ser enriquecida automaticamente com o nome do método chamador.</param>
        /// <param name="messageArgs">Argumentos opcionais para formatação da mensagem do log. São combinados com a mensagem fornecida (opcional).</param>
        /// <param name="eventId">O identificador único do evento associado ao log (opcional).</param>
        /// <param name="payload">
        /// Um objeto associado ao log, geralmente usado para fornecer informações adicionais relevantes ao contexto do log (opcional). 
        /// Se o objeto for uma exceção, ele será tratado como tal.
        /// </param>
        /// <param name="exception">Uma exceção associada ao log (opcional). Se fornecida, será tratada como parte do log.</param>
        /// <param name="enrichers">
        /// Uma coleção de pares chave/valor que fornecem dados adicionais para enriquecer o escopo do log (opcional).
        /// </param>
        /// <param name="callerMethodName">
        /// O nome do método chamador obtido automaticamente pelo compilador, usado para enriquecer a mensagem do log (opcional).
        /// </param>
        /// <returns>
        /// Uma string contendo informações sobre o log gerado, incluindo o nível do log (<see cref="LogLevel"/>),
        /// um identificador único (LogId) e o timestamp em milissegundos UTC no momento do registro.
        /// </returns>
        /// <remarks>
        /// O método verifica as configurações em <see cref="LogConfiguration"/>.<br/>
        /// Este método cria um escopo de log enriquecido com metadados adicionais, incluindo:<br/>
        /// <br/>
        /// - LogId: Um identificador único para a instância do log.<br/>
        /// - UnixTimeMilliseconds: Timestamp em milissegundos UTC.<br/>
        /// - Payload: Dados adicionais associados ao log, se fornecidos.<br/>
        /// <br/>
        /// O escopo é automaticamente encerrado ao término do método, garantindo o gerenciamento correto dos recursos.
        /// 
        /// O nome do método chamador (obtido através de <see cref="CallerMemberNameAttribute"/>) pode ser automaticamente prefixado à mensagem do log.
        /// 
        /// Exemplo de uso:
        /// <code>
        /// logger.ApplyLog(
        ///     logLevel: LogLevel.Warning,
        ///     message: "Ocorreu um erro durante o processamento. {id}",
        ///     messageArgs: [Guid.NewGuid()],
        ///     eventId: new EventId(101, "ErroProcessamento"),
        ///     exception: new InvalidOperationException("Operação inválida"),
        ///     enrichers:
        ///     [
        ///         new("UserId", 12345),
        ///         new("CorrelationId", Guid.NewGuid())
        ///     ]);
        /// </code>
        /// </remarks>
        public static string ApplyLog(
            this ILogger logger,
            LogLevel logLevel,
            string? message,
            object?[]? messageArgs = null,
            EventId? eventId = null,
            object? payload = null,
            Exception? exception = null,
            List<KeyValuePair<string, object>>? enrichers = null,
            [CallerMemberName] string? callerMethodName = null)
            => logger.InternalLog(
                logLevel: logLevel,
                message: message,
                messageArgs: messageArgs,
                eventId: eventId,
                payload: payload,
                exception: exception,
                enrichers: enrichers,
                callerMethodName: callerMethodName);

        /// <summary>
        /// Registra um log com escopo enriquecido, associando metadados adicionais ao log, e retorna uma string representando as informações do log gerado.
        /// </summary>
        /// <typeparamref name="TCategoryName"/>O tipo especificado da classe que está utilizando o logger.
        /// <param name="logger">A instância de <see cref="ILogger"/> usada para registrar o log.</param>
        /// <param name="logLevel">O nível de severidade do log (<see cref="LogLevel"/>).</param>
        /// <param name="message">A mensagem que será registrada no log. Pode ser enriquecida automaticamente com o nome do método chamador.</param>
        /// <param name="messageArgs">Argumentos opcionais para formatação da mensagem do log. São combinados com a mensagem fornecida (opcional).</param>
        /// <param name="eventId">O identificador único do evento associado ao log (opcional).</param>
        /// <param name="payload">
        /// Um objeto associado ao log, geralmente usado para fornecer informações adicionais relevantes ao contexto do log (opcional). 
        /// Se o objeto for uma exceção, ele será tratado como tal.
        /// </param>
        /// <param name="exception">Uma exceção associada ao log (opcional). Se fornecida, será tratada como parte do log.</param>
        /// <param name="enrichers">
        /// Uma coleção de pares chave/valor que fornecem dados adicionais para enriquecer o escopo do log (opcional).
        /// </param>
        /// <param name="callerMethodName">
        /// O nome do método chamador obtido automaticamente pelo compilador, usado para enriquecer a mensagem do log (opcional).
        /// </param>
        /// <returns>
        /// Uma string contendo informações sobre o log gerado, incluindo o nível do log (<see cref="LogLevel"/>),
        /// um identificador único (LogId) e o timestamp em milissegundos UTC no momento do registro.
        /// </returns>
        /// <remarks>
        /// O método verifica as configurações em <see cref="LogConfiguration"/>.<br/>
        /// Este método cria um escopo de log enriquecido com metadados adicionais, incluindo:<br/>
        /// <br/>
        /// - LogId: Um identificador único para a instância do log.<br/>
        /// - UnixTimeMilliseconds: Timestamp em milissegundos UTC.<br/>
        /// - Payload: Dados adicionais associados ao log, se fornecidos.<br/>
        /// <br/>
        /// O escopo é automaticamente encerrado ao término do método, garantindo o gerenciamento correto dos recursos.
        /// 
        /// O nome do método chamador (obtido através de <see cref="CallerMemberNameAttribute"/>) pode ser automaticamente prefixado à mensagem do log.
        /// 
        /// Exemplo de uso:
        /// <code>
        /// logger.ApplyLog(
        ///     logLevel: LogLevel.Warning,
        ///     message: "Ocorreu um erro durante o processamento. {id}",
        ///     messageArgs: [Guid.NewGuid()],
        ///     eventId: new EventId(101, "ErroProcessamento"),
        ///     exception: new InvalidOperationException("Operação inválida"),
        ///     enrichers:
        ///     [
        ///         new("UserId", 12345),
        ///         new("CorrelationId", Guid.NewGuid())
        ///     ]);
        /// </code>
        /// </remarks>
        public static string ApplyLog<TCategoryName>(
            this ILogger<TCategoryName> logger,
            LogLevel logLevel,
            string? message,
            object?[]? messageArgs = null,
            EventId? eventId = null,
            object? payload = null,
            Exception? exception = null,
            List<KeyValuePair<string, object>>? enrichers = null,
            [CallerMemberName] string? callerMethodName = null)
        {
            var allEnrichers = new List<KeyValuePair<string, object>>()
            {
                new("Class", typeof(TCategoryName).Name)
            };

            if (enrichers.IsNotNullOrEmpty())
                allEnrichers.AddRange(enrichers);

            return logger.InternalLog(
                logLevel: logLevel,
                message: message,
                messageArgs: messageArgs,
                eventId: eventId,
                payload: payload,
                exception: exception,
                enrichers: allEnrichers,
                callerMethodName: callerMethodName);
        }
        #endregion

        #region Public Methods/Operators TransactionId
        /// <summary>
        /// Registra um log com escopo enriquecido, associando metadados adicionais ao log, e retorna uma string representando as informações do log gerado.
        /// </summary>
        /// <param name="logger">A instância de <see cref="ILogger"/> usada para registrar o log.</param>
        /// <param name="logLevel">O nível de severidade do log (<see cref="LogLevel"/>).</param>
        /// <param name="transactionId">O identificador da transação relacionada ao log.</param>
        /// <param name="message">A mensagem que será registrada no log. Pode ser enriquecida automaticamente com o nome do método chamador.</param>
        /// <param name="messageArgs">Argumentos opcionais para formatação da mensagem do log. São combinados com a mensagem fornecida (opcional).</param>
        /// <param name="eventId">O identificador único do evento associado ao log (opcional).</param>
        /// <param name="payload">
        /// Um objeto associado ao log, geralmente usado para fornecer informações adicionais relevantes ao contexto do log (opcional). 
        /// Se o objeto for uma exceção, ele será tratado como tal.
        /// </param>
        /// <param name="exception">Uma exceção associada ao log (opcional). Se fornecida, será tratada como parte do log.</param>
        /// <param name="enrichers">
        /// Uma coleção de pares chave/valor que fornecem dados adicionais para enriquecer o escopo do log (opcional).
        /// </param>
        /// <param name="callerMethodName">
        /// O nome do método chamador obtido automaticamente pelo compilador, usado para enriquecer a mensagem do log (opcional).
        /// </param>
        /// <remarks>
        /// O método verifica as configurações em <see cref="LogConfiguration"/>.<br/>
        /// </remarks>
        /// <returns>
        /// Uma string contendo informações sobre o log gerado, incluindo o nível de log, o identificador único do log e o timestamp em milissegundos UTC.
        /// </returns>
        public static string ApplyLog(
            this ILogger logger,
            LogLevel logLevel,
            Guid transactionId,
            string? message,
            object?[]? messageArgs = null,
            EventId? eventId = null,
            object? payload = null,
            Exception? exception = null,
            IEnumerable<KeyValuePair<string, object>>? enrichers = null,
            [CallerMemberName] string? callerMethodName = null)
        {
            var allEnrichers = new List<KeyValuePair<string, object>>()
            {
                new("TransactionId", transactionId.ToString())
            };
            if (enrichers.IsNotNullOrEmpty())
                allEnrichers.AddRange(enrichers);

            return logger.InternalLog(
                logLevel: logLevel,
                message: message,
                messageArgs: messageArgs,
                eventId: eventId,
                payload: payload,
                exception: exception,
                enrichers: allEnrichers,
                callerMethodName: callerMethodName);
        }

        /// <summary>
        /// Registra um log com escopo enriquecido, associando metadados adicionais ao log, e retorna uma string representando as informações do log gerado.
        /// </summary>
        /// <typeparamref name="TCategoryName"/>O tipo especificado da classe que está utilizando o logger.
        /// <param name="logger">A instância de <see cref="ILogger"/> usada para registrar o log.</param>
        /// <param name="logLevel">O nível de severidade do log (<see cref="LogLevel"/>).</param>
        /// <param name="transactionId">O identificador da transação relacionada ao log.</param>
        /// <param name="message">A mensagem que será registrada no log. Pode ser enriquecida automaticamente com o nome do método chamador.</param>
        /// <param name="messageArgs">Argumentos opcionais para formatação da mensagem do log. São combinados com a mensagem fornecida (opcional).</param>
        /// <param name="eventId">O identificador único do evento associado ao log (opcional).</param>
        /// <param name="payload">
        /// Um objeto associado ao log, geralmente usado para fornecer informações adicionais relevantes ao contexto do log (opcional). 
        /// Se o objeto for uma exceção, ele será tratado como tal.
        /// </param>
        /// <param name="exception">Uma exceção associada ao log (opcional). Se fornecida, será tratada como parte do log.</param>
        /// <param name="enrichers">
        /// Uma coleção de pares chave/valor que fornecem dados adicionais para enriquecer o escopo do log (opcional).
        /// </param>
        /// <param name="callerMethodName">
        /// O nome do método chamador obtido automaticamente pelo compilador, usado para enriquecer a mensagem do log (opcional).
        /// </param>
        /// <remarks>
        /// O método verifica as configurações em <see cref="LogConfiguration"/>.<br/>
        /// </remarks>
        /// <returns>
        /// Uma string contendo informações sobre o log gerado, incluindo o nível de log, o identificador único do log e o timestamp em milissegundos UTC.
        /// </returns>
        public static string ApplyLog<TCategoryName>(
            this ILogger<TCategoryName> logger,
            LogLevel logLevel,
            Guid transactionId,
            string? message,
            object?[]? messageArgs = null,
            EventId? eventId = null,
            object? payload = null,
            Exception? exception = null,
            IEnumerable<KeyValuePair<string, object>>? enrichers = null,
            [CallerMemberName] string? callerMethodName = null)
        {
            var allEnrichers = new List<KeyValuePair<string, object>>()
            {
                new("TransactionId", transactionId.ToString()),
                new("Class", typeof(TCategoryName).Name)
            };

            if (enrichers.IsNotNullOrEmpty())
                allEnrichers.AddRange(enrichers);

            return logger.InternalLog(
                logLevel: logLevel,
                message: message,
                messageArgs: messageArgs,
                eventId: eventId,
                payload: payload,
                exception: exception,
                enrichers: allEnrichers,
                callerMethodName: callerMethodName);
        }
        #endregion

        #region Private Methods/Operators
        /// <summary>
        /// Registra um log com escopo enriquecido, associando metadados adicionais ao log, e retorna uma string representando as informações do log gerado.
        /// </summary>
        /// <param name="logger">A instância de <see cref="ILogger"/> usada para registrar o log.</param>
        /// <param name="logLevel">O nível de severidade do log (<see cref="LogLevel"/>).</param>
        /// <param name="message">A mensagem que será registrada no log. Pode ser enriquecida automaticamente com o nome do método chamador.</param>
        /// <param name="messageArgs">Argumentos opcionais para formatação da mensagem do log. São combinados com a mensagem fornecida (opcional).</param>
        /// <param name="eventId">O identificador único do evento associado ao log (opcional).</param>
        /// <param name="payload">
        /// Um objeto associado ao log, geralmente usado para fornecer informações adicionais relevantes ao contexto do log (opcional). 
        /// Se o objeto for uma exceção, ele será tratado como tal.
        /// </param>
        /// <param name="exception">Uma exceção associada ao log (opcional). Se fornecida, será tratada como parte do log.</param>
        /// <param name="enrichers">
        /// Uma coleção de pares chave/valor que fornecem dados adicionais para enriquecer o escopo do log (opcional).
        /// </param>
        /// <param name="callerMethodName">
        /// O nome do método chamador obtido automaticamente pelo compilador, usado para enriquecer a mensagem do log (opcional).
        /// </param>
        /// <returns>
        /// Uma string contendo informações sobre o log gerado, incluindo o nível do log (<see cref="LogLevel"/>),
        /// um identificador único (LogId) e o timestamp em milissegundos UTC no momento do registro.
        /// </returns>
        private static string InternalLog(
            this ILogger logger,
            LogLevel logLevel,
            string? message,
            object?[]? messageArgs = null,
            EventId? eventId = null,
            object? payload = null,
            Exception? exception = null,
            List<KeyValuePair<string, object>>? enrichers = null,
            string? callerMethodName = null)
        {
            messageArgs ??= [];
            enrichers ??= [];

            var logId = Guid.NewGuid();
            var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var count = enrichers.Count + 2;
            if (LogConfiguration.IncludeAdditionalData)
            {
                if (payload is not null) count++;
                if (callerMethodName.IsNotNullOrEmpty()) count++;
            }

            var allEnrichers = new List<KeyValuePair<string, object>>(count)
            {
                new(ApplyScopePrefix("LogId"), logId),
                new(ApplyScopePrefix("UnixTimeMilliseconds"), unixTimeMilliseconds)
            };

            if (LogConfiguration.IncludeAdditionalData)
            {
                if (callerMethodName.IsNotNullOrEmpty())
                    allEnrichers.Add(new(ApplyScopePrefix("CallerMethodName"), callerMethodName));

                if (payload is not null)
                    allEnrichers.Add(new(ApplyScopePrefix("Payload"), payload));
            }

            if (enrichers.Count > 0)
                foreach (var enricher in enrichers)
                    allEnrichers.Add(new(ApplyScopePrefix(enricher.Key), enricher.Value));

            using (logger.BeginScope(allEnrichers))
                logger.Log(logLevel, eventId ?? default, exception, ApplyMessageArgsPrefix(message), messageArgs);

            return $"[{logLevel}]-[{logId}]-[{unixTimeMilliseconds}]";
        }

        /// <summary>
        /// Aplica o prefixo de escopo definido na configuração global ao nome da chave, caso esteja habilitado.
        /// </summary>
        /// <param name="key">A chave que será analisada e possivelmente prefixada.</param>
        /// <returns>
        /// A chave com o prefixo de escopo aplicado, caso necessário, ou a chave original se o prefixo já estiver presente ou a configuração estiver desativada.
        /// </returns>
        /// <remarks>
        /// O método verifica se o uso de prefixos está habilitado em <see cref="LogConfiguration.UseScopePrefix"/>.<br/>
        /// Se habilitado, adiciona o prefixo configurado em <see cref="LogConfiguration.ScopePrefix"/> à chave fornecida, caso o prefixo ainda não esteja presente.
        /// </remarks>
        private static string ApplyScopePrefix(string key)
        {
            if (!LogConfiguration.UseScopePrefix)
                return key;

            var prefix = LogConfiguration.ScopePrefix + '.';
            return key.StartsWith(prefix, StringComparison.Ordinal)
                ? key
                : prefix + key;
        }

        /// <summary>
        /// Adiciona o prefixo "MessageArgs." aos placeholders de uma mensagem, caso a configuração global <see cref="LogConfiguration.UseMessageArgsPrefix"/> esteja habilitada.
        /// </summary>
        /// <param name="message">A mensagem original contendo placeholders no formato <c>{Placeholder}</c>.</param>
        /// <returns>
        /// A mensagem modificada com o prefixo "MessageArgs." aplicado a todos os placeholders, ou a mensagem original
        /// se a configuração <see cref="LogConfiguration.UseMessageArgsPrefix"/> estiver desativada ou a mensagem for nula/vazia.
        /// </returns>
        /// <remarks>
        /// O método localiza dinamicamente todos os placeholders dentro de chaves (<c>{}</c>) e adiciona o prefixo "MessageArgs."
        /// antes de cada nome de placeholder. Se a mensagem não contiver placeholders, ela é retornada inalterada.
        /// </remarks>
        /// <example>
        /// Mensagem original: <c>"[{StatusCode}:{Method}] {Source}"</c> <br/>
        /// Mensagem resultante: <c>"[{MessageArgsStatusCode}:{MessageArgsMethod}] {MessageArgsSource}"</c>
        /// </example>
        private static string? ApplyMessageArgsPrefix(string? message)
        {
            if (!LogConfiguration.UseMessageArgsPrefix || string.IsNullOrWhiteSpace(message))
                return message;

            var estimatedCapacity = message.Length + message.Count(c => c == '{') * "MessageArgs".Length;
            var updatedMessage = new StringBuilder(estimatedCapacity);

            for (int i = 0; i < message.Length; i++)
                if (message[i] == '{')
                    updatedMessage
                        .Append('{')
                        .Append("MessageArgs");
                else
                    updatedMessage
                        .Append(message[i]);

            return updatedMessage.ToString();
        }
        #endregion
    }
}