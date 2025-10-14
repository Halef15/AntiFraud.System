using AntiFraud.System.Application.Utilities;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AntiFraud.System.Infrastructure.Observability.Settings
{
    /// <summary>
    /// Representa as configurações necessárias para a integração com um servidor OTLP (OpenTelemetry Protocol).<br/>
    /// Inclui informações essenciais como endpoint do servidor, identificação do serviço e atributos padrão do ambiente de execução.
    /// </summary>
    public record OtlpSettings
    {
        #region Variables
        /// <summary>
        /// Dicionário interno de atributos padrão que serão enviados junto com as métricas e rastreamentos.
        /// </summary>
        private readonly Dictionary<string, object> _attributes;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa uma nova instância de <see cref="OtlpSettings"/> com o endpoint OTLP e o nome do serviço.<br/>
        /// Preenche também diversos atributos padrão, como informações sobre o host, sistema operacional e runtime.
        /// </summary>
        /// <param name="serviceName">
        /// O nome do serviço que será utilizado para identificar o remetente de métricas ou rastreamentos.<br/>
        /// Deve ser uma string não vazia e não nula.
        /// </param>
        /// <exception cref="ArgumentException">Lançada se <paramref name="serviceName"/> for nulo ou vazio.</exception>
        public OtlpSettings(string serviceName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(serviceName, nameof(serviceName));

            ServiceName = serviceName;
            ServiceNamespace = Assembly.GetEntryAssembly()?.GetName().Name ?? "undefined";
            ServiceVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "undefined";

            _attributes = new Dictionary<string, object>
            {
                ["host.name"] = Environment.MachineName,
                ["host.username"] = Environment.UserName,
                ["host.ip"] = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "undefined",

                ["service.name"] = ServiceName,
                ["service.namespace"] = ServiceNamespace,
                ["service.version"] = ServiceVersion,

                ["os.kernel.version"] = Environment.OSVersion.VersionString,
                ["os.architecture"] = RuntimeInformation.OSArchitecture.ToString(),
                ["os.platform"] = RuntimeInformation.OSDescription,

                ["runtime.framework"] = RuntimeInformation.FrameworkDescription,
                ["runtime.platform"] = RuntimeInformation.RuntimeIdentifier
            };
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Obtém o nome do serviço que será utilizado para identificar o remetente de métricas ou rastreamentos.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Obtém o namespace do serviço, utilizado para categorizar ou organizar serviços dentro do OTLP.
        /// </summary>
        public string ServiceNamespace { get; }

        /// <summary>
        /// Obtém a versão do serviço, utilizada para categorizar ou organizar serviços dentro do OTLP.
        /// </summary>
        public string ServiceVersion { get; }

        /// <summary>
        /// Obtém a URL do endpoint do servidor OTLP.
        /// Pode ser um endereço IP ou um nome de domínio completo.
        /// </summary>
        public required string Endpoint { get; init; }

        /// <summary>
        /// Obtém ou define uma coleção de atributos adicionais que podem ser enviados junto com as métricas ou rastreamentos.
        /// Caso seja fornecida uma coleção, seus valores serão mesclados aos atributos padrão.
        /// </summary>
        /// <remarks>
        /// Esta propriedade permite adicionar ou substituir atributos padrão.
        /// Por exemplo, se já existir um atributo "host.name" e a coleção fornecida também contiver "host.name",
        /// o valor será sobrescrito.
        /// </remarks>
        public IReadOnlyCollection<KeyValuePair<string, object>> Attributes
        {
            get => _attributes;
            init
            {
                if (value is not null)
                    foreach (var item in value)
                        _attributes[item.Key.ToLower()] = item.Value;
            }
        }

        /// <summary>
        /// Obtém ou define uma lista de nomes de medidores (meters) a serem utilizados para coletar métricas.
        /// Caso seja vazio, o sistema tentará coletar métricas padrão.
        /// </summary>
        public string[] MeterNames { get; init; } = [];

        /// <summary>
        /// Obtém ou define uma lista de nomes de fontes de rastreamento (tracing sources) a serem utilizadas para coletar rastreamentos.
        /// Caso seja vazio, o sistema tentará coletar rastreamentos padrão.
        /// </summary>
        public string[] TracingSourceNames { get; init; } = [];
        #endregion
    }
}