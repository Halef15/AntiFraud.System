using AntiFraud.System.Application.Utilities;
using AntiFraud.System.Infrastructure.Observability.Settings;
using AntiFraud.System.Infrastructure.Observability.Tracers;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Sinks.OpenTelemetry;
using System.Diagnostics;

namespace AntiFraud.System.Infrastructure.Observability
{
    /// <summary>
    /// Extensões para configurar observabilidade e logging usando OpenTelemetry e Serilog no AntiFraud.
    /// </summary>
    public static class ObservabilityExtensions
    {
        #region Public Methods/Operators Serilog
        /// <summary>
        /// Configura o Serilog como provedor de logging com suporte para OTLP (OpenTelemetry Protocol).
        /// Essa configuração adiciona enriquecimento de logs com atributos personalizados, 
        /// correlação e integração com OTLP para exportação de logs estruturados.
        /// </summary>
        /// <param name="builder">O construtor da aplicação web (<see cref="WebApplicationBuilder"/>).</param>
        /// <param name="loggerConfig">
        /// Uma ação opcional para customizar a configuração do <see cref="HostBuilderContext"/>, <see cref="IServiceProvider"/> e <see cref="LoggerConfiguration"/>.<br/>
        /// Se não for fornecida, será usada a configuração padrão.
        /// </param>
        /// <returns>O <see cref="WebApplicationBuilder"/> configurado com o Serilog e suporte a OTLP.</returns>
        /// <remarks>
        /// Este método deve ser chamado durante a configuração inicial do aplicativo para configurar o Serilog como o provedor de logs padrão.
        /// 
        /// Exemplo de uso:
        /// <code>
        /// var builder = WebApplication.CreateBuilder(args);
        /// builder.AddAntiFraudSerilogOtlp((context, service, logger) => logger.AddAntiFraudSerilogOtlp(otlpSettings));
        /// </code>
        /// </remarks>
        public static WebApplicationBuilder AddAntiFraudSerilogOtlp(
            this WebApplicationBuilder builder,
            Action<HostBuilderContext, IServiceProvider, LoggerConfiguration>? loggerConfig = null)
        {
            builder.Logging.ClearProviders();
            builder.Host.AddAntiFraudSerilog(loggerConfig);
            return builder;
        }

        /// <summary>
        /// Configura o Serilog como provedor de logging integrado ao OpenTelemetry Protocol (OTLP), 
        /// permitindo a exportação de logs estruturados com suporte a atributos personalizados e enriquecimento de contexto.
        /// </summary>
        /// <param name="builder">
        /// O construtor do host da aplicação (<see cref="IHostBuilder"/>) onde o Serilog será configurado.
        /// </param>
        /// <param name="loggerConfig">
        /// Uma ação opcional para customizar a configuração do <see cref="HostBuilderContext"/>, <see cref="IServiceProvider"/> e <see cref="LoggerConfiguration"/>.<br/>
        /// Se não for fornecida, será usada a configuração padrão.
        /// </param>
        /// <returns>
        /// A instância configurada do <see cref="IHostBuilder"/> com o Serilog e suporte ao OTLP.
        /// </returns>
        /// <remarks>
        /// Este método configura o Serilog para usar OTLP como destino de exportação de logs estruturados.
        /// Ele inclui:
        /// <list type="bullet">
        /// <item>Leitura da configuração do appsettings.json.</item>
        /// <item>Enriquecimento de logs com detalhes do contexto da aplicação, como processo, thread e IDs de correlação.</item>
        /// <item>Integração com OTLP para exportar logs para uma plataforma de observabilidade.</item>
        /// </list>
        /// <para>
        /// Para personalizar o comportamento do Serilog, use o parâmetro <paramref name="loggerConfig"/> para passar uma ação de configuração.
        /// </para>
        /// <example>
        /// Exemplo de uso:
        /// <code>
        /// var hostBuilder = Host.CreateDefaultBuilder(args);
        /// hostBuilder.AddAntiFraudSerilog((context, service, logger) => logger.AddAntiFraudSerilogOtlp(otlpSettings));
        /// </code>
        /// </example>
        /// </remarks>
        public static IHostBuilder AddAntiFraudSerilog(
            this IHostBuilder builder,
            Action<HostBuilderContext, IServiceProvider, LoggerConfiguration>? loggerConfig = null)
        {
            builder.UseSerilog(
                writeToProviders: false,
                preserveStaticLogger: false,
                configureLogger: (context, service, logger) =>
                {
                    logger
                        // Lê a configuração do appsettings.json
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(service)

                        // Configuração do "MinimumLevel" de log
                        .MinimumLevel.Warning()
                        .MinimumLevel.Override("AntiFraud", LogEventLevel.Information)

                        // Enriquecendo os logs com informações adicionais
                        .Enrich.FromLogContext()
                        .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                            .WithIgnoreStackTraceAndTargetSiteExceptionFilter()
                            .WithDefaultDestructurers())
                        .Enrich.WithDemystifiedStackTraces()
                        .Enrich.WithProcessId()
                        .Enrich.WithProcessName()
                        .Enrich.WithThreadId()
                        .Enrich.WithThreadName()
                        .Enrich.WithClientIp()
                        .Enrich.WithCorrelationId();

                    loggerConfig?.Invoke(context, service, logger);
                });
            return builder;
        }

        /// <summary>
        /// Adiciona configurações para envio de logs ao OpenTelemetry Protocol (OTLP) em uma instância do <see cref="LoggerConfiguration"/>.
        /// </summary>
        /// <param name="loggerConfig">
        /// A instância do <see cref="LoggerConfiguration"/> onde o OTLP será configurado.
        /// </param>
        /// <param name="otlpSettings">
        /// As configurações específicas do OTLP, incluindo o endpoint, protocolo e atributos de recurso.
        /// </param>
        /// <returns>
        /// A instância atualizada do <see cref="LoggerConfiguration"/> com o OTLP configurado.
        /// </returns>
        /// <remarks>
        /// Este método configura o envio de logs para um endpoint OTLP utilizando o protocolo gRPC.
        /// Ele permite:
        /// <list type="bullet">
        /// <item>Configurar o endpoint e protocolo OTLP.</item>
        /// <item>Definir atributos de recurso para enriquecer os logs enviados.</item>
        /// <item>Especificar quais dados adicionais dos logs serão incluídos, como:
        /// <list type="number">
        /// <item>Texto do template da mensagem.</item>
        /// <item>Hash MD5 do template da mensagem.</item>
        /// <item>Renderizações do template da mensagem.</item>
        /// <item>Identificadores de rastreamento (<c>SpanId</c> e <c>TraceId</c>).</item>
        /// <item>Atributos obrigatórios especificados no OTLP.</item>
        /// </list>
        /// </item>
        /// </list>
        /// O envio de logs é assíncrono para melhorar a performance.
        /// </remarks>
        public static LoggerConfiguration AddAntiFraudSerilogOtlp(
            this LoggerConfiguration loggerConfig,
            OtlpSettings otlpSettings)
        {
            loggerConfig.WriteTo.Async(asyncOptions =>
            {
                asyncOptions.OpenTelemetry(options =>
                {
                    // Configuração do endpoint OTLP
                    options.Endpoint = otlpSettings.Endpoint;
                    options.Protocol = OtlpProtocol.Grpc;
                    options.ResourceAttributes = otlpSettings.Attributes.ToDictionary();

                    // Configurações para incluir atributos
                    options.IncludedData = IncludedData.MessageTemplateTextAttribute
                        | IncludedData.MessageTemplateMD5HashAttribute
                        | IncludedData.MessageTemplateRenderingsAttribute
                        | IncludedData.SpanIdField
                        | IncludedData.SpecRequiredResourceAttributes
                        | IncludedData.TraceIdField;
                });
            });
            return loggerConfig;
        }
        #endregion

        #region Public Methods/Operators OpenTelemetry
        /// <summary>
        /// Configura serviços de observabilidade com suporte a métricas e rastreamento usando OpenTelemetry,
        /// exportando os dados via OTLP (OpenTelemetry Protocol).
        /// </summary>
        /// <param name="services">A coleção de serviços da aplicação (<see cref="IServiceCollection"/>).</param>
        /// <param name="otlpSettings">As configurações de OTLP, incluindo endpoint, atributos e fontes de rastreamento.</param>
        /// <returns>A coleção de serviços configurada com suporte a observabilidade.</returns>
        /// <remarks>
        /// Este método configura o OpenTelemetry com:
        /// <list type="bullet">
        /// <item><description>Recursos personalizados definidos em <see cref="OtlpSettings"/>.</description></item>
        /// <item><description>Suporte a métricas e rastreamento utilizando os builders fornecidos pela AntiFraud.</description></item>
        /// <item><description>Exportação de dados para o endpoint OTLP configurado.</description></item>
        /// </list>
        /// Exemplo de uso:
        /// <code>
        /// services.AddAntiFraudObservabilityOtlp(new OtlpSettings
        /// {
        ///     Endpoint = "http://localhost:4317",
        /// });
        /// </code>
        /// </remarks>
        public static IServiceCollection AddAntiFraudObservabilityOtlp(
            this IServiceCollection services,
            OtlpSettings otlpSettings)
        {
            services
                .AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddAntiFraudOtlpResourceBuilder(otlpSettings))
                .WithMetrics(metrics => metrics
                    .AddAntiFraudOtlpMeterBuilder(otlpSettings))
                .WithTracing(tracing => tracing
                    .AddAntiFraudOtlpTracerBuilder(otlpSettings))
                .UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(otlpSettings.Endpoint));
            return services;
        }

        /// <summary>
        /// Configura serviços de observabilidade com suporte a métricas e rastreamento usando OpenTelemetry,
        /// permitindo configurar opções adicionais do exportador OTLP.
        /// </summary>
        /// <param name="services">A coleção de serviços da aplicação (<see cref="IServiceCollection"/>).</param>
        /// <param name="otlpSettings">As configurações de OTLP, incluindo endpoint, atributos e fontes de rastreamento.</param>
        /// <param name="exporterOptions">Uma ação para configurar as opções do exportador OTLP (<see cref="OtlpExporterOptions"/>).</param>
        /// <returns>A coleção de serviços configurada com suporte a observabilidade.</returns>
        /// <remarks>
        /// Este método oferece maior flexibilidade ao permitir que as opções do exportador OTLP sejam configuradas.
        /// <list type="bullet">
        /// <item><description>Recursos personalizados definidos em <see cref="OtlpSettings"/>.</description></item>
        /// <item><description>Suporte a métricas e rastreamento utilizando os builders fornecidos pela AntiFraud.</description></item>
        /// <item><description>Exportação de dados configurada dinamicamente através de <paramref name="exporterOptions"/>.</description></item>
        /// </list>
        /// Exemplo de uso:
        /// <code>
        /// services.AddAntiFraudObservabilityOtlp(new OtlpSettings
        /// {
        ///     Endpoint = "http://localhost:4317",
        /// }, options =>
        /// {
        ///     options.Endpoint = new Uri("http://custom-endpoint:4317");
        ///     options.Headers = new Dictionary&lt;string, string&gt; { ["Authorization"] = "Bearer my-token" };
        /// });
        /// </code>
        /// </remarks>
        public static IServiceCollection AddAntiFraudObservabilityOtlp(
            this IServiceCollection services,
            OtlpSettings otlpSettings,
            Action<OtlpExporterOptions> exporterOptions)
        {
            ArgumentNullException.ThrowIfNull(exporterOptions, nameof(exporterOptions));
            services
                .AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddAntiFraudOtlpResourceBuilder(otlpSettings))
                .WithMetrics(metrics => metrics
                    .AddAntiFraudOtlpMeterBuilder(otlpSettings)
                    .AddOtlpExporter(exporterOptions))
                .WithTracing(tracing => tracing
                    .AddAntiFraudOtlpTracerBuilder(otlpSettings)
                    .AddOtlpExporter(exporterOptions));
            return services;
        }

        /// <summary>
        /// Configura o <see cref="ResourceBuilder"/> para incluir informações sobre o serviço,
        /// atributos personalizados e detectores de ambiente.
        /// </summary>
        /// <param name="builder">O construtor de recursos do OpenTelemetry (<see cref="ResourceBuilder"/>).</param>
        /// <param name="otlpSettings">As configurações de OTLP contendo informações sobre o serviço e atributos personalizados.</param>
        /// <returns>O <see cref="ResourceBuilder"/> configurado.</returns>
        /// <remarks>
        /// Este método adiciona as seguintes informações ao <see cref="ResourceBuilder"/>:
        /// <list type="bullet">
        /// <item><description>Informações do serviço, como nome, namespace, versão e ID da instância.</description></item>
        /// <item><description>Atributos personalizados definidos em <see cref="OtlpSettings.Attributes"/>.</description></item>
        /// <item><description>Detectores de ambiente, incluindo Azure, contêineres, SO e variáveis de ambiente.</description></item>
        /// </list>
        /// Exemplo de uso:
        /// <code>
        /// builder.AddAntiFraudOtlpResourceBuilder(new OtlpSettings
        /// {
        ///     ServiceName = "MyService",
        ///     ServiceNamespace = "MyNamespace",
        ///     ServiceVersion = "1.0.0",
        ///     Attributes = new Dictionary&lt;string, object&gt;
        ///     {
        ///         ["environment"] = "Production"
        ///     }
        /// });
        /// </code>
        /// </remarks>
        public static ResourceBuilder AddAntiFraudOtlpResourceBuilder(
            this ResourceBuilder builder,
            OtlpSettings otlpSettings)
        {
            builder
                .AddService(
                    serviceName: otlpSettings.ServiceName,
                    serviceNamespace: otlpSettings.ServiceNamespace,
                    serviceVersion: otlpSettings.ServiceVersion,
                    serviceInstanceId: Environment.MachineName
                )
                .AddTelemetrySdk()
                .AddAzureAppServiceDetector()
                .AddAzureContainerAppsDetector()
                .AddAzureVMDetector()
                .AddHostDetector()
                .AddContainerDetector()
                .AddEnvironmentVariableDetector()
                .AddProcessDetector()
                .AddProcessRuntimeDetector()
                .AddOperatingSystemDetector();

            if (otlpSettings.Attributes.IsNotNullOrEmpty())
                builder.AddAttributes(otlpSettings.Attributes);

            return builder;
        }

        /// <summary>
        /// Configura o <see cref="MeterProviderBuilder"/> para incluir medidores e instrumentação padrão.
        /// </summary>
        /// <param name="builder">O provedor de medidores do OpenTelemetry (<see cref="MeterProviderBuilder"/>).</param>
        /// <param name="otlpSettings">As configurações de OTLP contendo nomes de medidores.</param>
        /// <returns>O <see cref="MeterProviderBuilder"/> configurado.</returns>
        /// <remarks>
        /// Este método configura a instrumentação de métricas, incluindo:
        /// <list type="bullet">
        /// <item><description>Medidores personalizados definidos em <see cref="OtlpSettings.MeterNames"/>.</description></item>
        /// <item><description>Instrumentação padrão para ASP.NET Core, processos e runtime.</description></item>
        /// </list>
        /// Exemplo de uso:
        /// <code>
        /// builder.AddAntiFraudOtlpMeterBuilder(new OtlpSettings
        /// {
        ///     MeterNames = new[] { "MyServiceMeter" }
        /// });
        /// </code>
        /// </remarks>
        public static MeterProviderBuilder AddAntiFraudOtlpMeterBuilder(
            this MeterProviderBuilder builder,
            OtlpSettings otlpSettings)
        {
            if (otlpSettings.MeterNames.IsNotNullOrEmpty())
                builder.AddMeter(otlpSettings.MeterNames);

            return builder
                .AddAspNetCoreInstrumentation()
                .AddProcessInstrumentation()
                .AddRuntimeInstrumentation();
        }

        /// <summary>
        /// Configura o <see cref="TracerProviderBuilder"/> para rastreamento,
        /// incluindo instrumentação para ASP.NET Core, HttpClient e Entity Framework.
        /// </summary>
        /// <param name="builder">O provedor de rastreamento do OpenTelemetry (<see cref="TracerProviderBuilder"/>).</param>
        /// <param name="otlpSettings">As configurações de OTLP contendo nomes de fontes de rastreamento.</param>
        /// <returns>O <see cref="TracerProviderBuilder"/> configurado.</returns>
        /// <remarks>
        /// Este método adiciona suporte avançado para rastreamento, incluindo:
        /// <list type="bullet">
        /// <item><description>Fontes de rastreamento personalizadas definidas em <see cref="OtlpSettings.TracingSourceNames"/>.</description></item>
        /// <item><description>Instrumentação para ASP.NET Core, HttpClient e Entity Framework Core.</description></item>
        /// <item><description>Configuração para tratamento de exceções e amostragem "AlwaysOn".</description></item>
        /// </list>
        /// Exemplo de uso:
        /// <code>
        /// builder.AddAntiFraudOtlpTracerBuilder(new OtlpSettings
        /// {
        ///     TracingSourceNames = new[] { "MyService" }
        /// });
        /// </code>
        /// </remarks>
        public static TracerProviderBuilder AddAntiFraudOtlpTracerBuilder(
            this TracerProviderBuilder builder,
            OtlpSettings otlpSettings)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            if (otlpSettings.TracingSourceNames.IsNotNullOrEmpty())
                builder.AddSource(otlpSettings.TracingSourceNames);

            return builder
                .SetSampler(new AlwaysOnSampler())
                .SetErrorStatusOnException()
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = (httpContext) =>
                    {
                        var path = httpContext.Request.Path.Value ?? string.Empty;
                        return !(path.EndsWith(".css")
                        || path.EndsWith(".js")
                        || path.EndsWith(".ico")
                        || path.EndsWith(".json"));
                    };
                    options.EnrichWithHttpRequest = (activity, httpRequest) =>
                    {
                        activity.SetTag("request.protocol", httpRequest.Protocol);
                    };
                    options.EnrichWithException = (activity, exception) =>
                    {
                        activity.SetTag("exception.stackTrace", exception.StackTrace);
                        activity.SetTag("exception.source", exception.Source);
                    };
                })
                .AddHttpClientInstrumentation((options) =>
                {
                    options.RecordException = true;
                    options.EnrichWithException = (activity, exception) =>
                    {
                        activity.SetTag("exception.stackTrace", exception.StackTrace);
                        activity.SetTag("exception.source", exception.Source);
                    };
                })
                .AddEntityFrameworkCoreInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.SetDbStatementForStoredProcedure = true;
                    options.EnrichWithIDbCommand = (activity, command) =>
                    {
                        activity.DisplayName += $" - {command.CommandType}";
                    };
                });
        }
        #endregion

        #region Public Methods/Operators Tracing Interceptor
        /// <summary>
        /// Adiciona suporte ao interceptor de rastreamento de métodos (<see cref="MethodTracingInterceptor"/>) ao pipeline do aplicativo.
        /// </summary>
        /// <param name="builder">O construtor do aplicativo web (<see cref="WebApplicationBuilder"/>).</param>
        /// <param name="includeNamespaces">Uma lista opcional de namespaces a serem incluídos no rastreamento. Se não especificado, todos os namespaces serão incluídos.</param>
        /// <param name="excludeNamespaces">Uma lista opcional de namespaces a serem excluídos do rastreamento.</param>
        /// <returns>O construtor do aplicativo web com o suporte ao interceptor configurado.</returns>
        /// <remarks>
        /// Este método deve ser chamado **após** a criação do <see cref="WebApplicationBuilder"/> usando `WebApplication.CreateBuilder`, pois ele configura o uso da fábrica 
        /// <see cref="DynamicProxyServiceProviderFactory"/> necessária para aplicar os interceptores dinâmicos.
        /// 
        /// Adiciona suporte ao interceptor de rastreamento de métodos para capturar telemetria de execução de métodos, utilizando proxies dinâmicos para aplicar interceptores em tempo de execução.
        /// 
        /// <example>
        /// Exemplo de uso:
        /// <code>
        /// var builder = WebApplication.CreateBuilder(args);
        /// builder.AddAntiFraudMethodTracingInterceptor(
        ///     includeNamespaces: new[] { "MyApp*" },
        ///     excludeNamespaces: new[] { "MyApp.Services.Internal" });
        /// </code>
        /// </example>
        /// </remarks>
        public static WebApplicationBuilder AddAntiFraudMethodTracingInterceptor(
            this WebApplicationBuilder builder,
            IEnumerable<string>? includeNamespaces = default,
            IEnumerable<string>? excludeNamespaces = default)
        {
            builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
            builder.Services.AddAntiFraudMethodTracingInterceptor(includeNamespaces, excludeNamespaces);
            return builder;
        }

        /// <summary>
        /// Adiciona suporte ao interceptor de rastreamento de métodos (<see cref="MethodTracingInterceptor"/>) aos serviços do aplicativo.
        /// </summary>
        /// <param name="services">A coleção de serviços do aplicativo (<see cref="IServiceCollection"/>).</param>
        /// <param name="includeNamespaces">Uma lista opcional de namespaces a serem incluídos no rastreamento. Se não especificado, todos os namespaces serão incluídos.</param>
        /// <param name="excludeNamespaces">Uma lista opcional de namespaces a serem excluídos do rastreamento.</param>
        /// <returns>A coleção de serviços configurada.</returns>
        /// <remarks>
        /// Este método adiciona o interceptor de rastreamento de métodos aos serviços do aplicativo, permitindo capturar telemetria de execução de métodos.
        /// <example>
        /// Exemplo de uso:
        /// <code>
        /// builder.Services.AddAntiFraudMethodTracingInterceptor(
        ///     includeNamespaces: new[] { "MyApp*" },
        ///     excludeNamespaces: new[] { "MyApp.Services.Internal" });
        /// </code>
        /// </example>
        /// </remarks>
        public static IServiceCollection AddAntiFraudMethodTracingInterceptor(
            this IServiceCollection services,
            IEnumerable<string>? includeNamespaces = default,
            IEnumerable<string>? excludeNamespaces = default)
        {
            services.AddTransient<MethodTracingInterceptor>();
            services.ConfigureDynamicProxy(config =>
            {
                if (includeNamespaces.IsNotNullOrEmpty())
                    foreach (var ns in includeNamespaces)
                        config.Interceptors.AddTyped<MethodTracingInterceptor>(Predicates.ForNameSpace(ns));
                else
                    config.Interceptors.AddTyped<MethodTracingInterceptor>();

                if (excludeNamespaces.IsNotNullOrEmpty())
                    foreach (var ns in excludeNamespaces)
                        config.NonAspectPredicates.AddNamespace(ns);
            });

            return services;
        }
        #endregion
    }
}