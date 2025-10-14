using AspectCore.DynamicProxy;
using System.Diagnostics;

namespace AntiFraud.System.Infrastructure.Observability.Tracers
{
    /// <summary>
    /// Interceptor para rastreamento de execução de métodos utilizando OpenTelemetry.
    /// </summary>
    public class MethodTracingInterceptor : IInterceptor
    {
        #region Protected Properties
        /// <summary>
        /// Fonte de atividade utilizada para criar e rastrear atividades durante a execução de métodos.
        /// </summary>
        protected readonly ActivitySource _activitySource;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public MethodTracingInterceptor()
        {
            _activitySource = new ActivitySource("AntiFraud.Method");
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Indica se múltiplas instâncias do interceptor são permitidas. Sempre retorna <c>false</c>.
        /// </summary>
        public bool AllowMultiple => false;

        /// <summary>
        /// Indica se o interceptor será herdado por classes derivadas.
        /// </summary>
        public bool Inherited { get; set; } = false;

        /// <summary>
        /// Define a ordem de execução do interceptor.
        /// </summary>
        public int Order { get; set; } = 0;
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Executa o rastreamento da execução do método, criando uma atividade OpenTelemetry para capturar informações de telemetria.
        /// </summary>
        /// <param name="context">O contexto da interceptação, contendo informações sobre o método e o serviço.</param>
        /// <param name="next">Delegado que representa a próxima etapa no pipeline de interceptação.</param>
        /// <returns>Uma tarefa assíncrona que representa a execução do método interceptado.</returns>
        public virtual async Task Invoke(AspectContext context, AspectDelegate next)
        {
            using var activity = _activitySource.StartActivity(context.ProxyMethod.Name, ActivityKind.Internal);
            if (activity is null)
            {
                await next(context).ConfigureAwait(false);
                return;
            }

            if (!string.IsNullOrEmpty(activity.Id)
                && Activity.Current != null
                && string.IsNullOrEmpty(Activity.Current.ParentId)
                && activity.Id != Activity.Current.Id)
                Activity.Current.SetParentId(activity.Id);

            activity.SetTag("activity.spanId", activity.SpanId);
            activity.SetTag("activity.parentId", activity.ParentId);

            var declaringTypeName = context.ServiceMethod.DeclaringType?.Name ?? "unknown";
            activity.DisplayName = $"{declaringTypeName}.{context.ProxyMethod.Name}";
            activity.SetTag("method.class", declaringTypeName);
            activity.SetTag("method.name", context.ProxyMethod.Name);

            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                activity.SetTag("method.error", ex.Message);
                throw;
            }
            finally
            {
                activity.Stop();
            }
        }
        #endregion
    }
}