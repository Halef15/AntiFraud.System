namespace AntiFraud.System.Infrastructure.Observability.Settings
{
    /// <summary>
    /// Configurações globais para gerenciamento de logs na aplicação.
    /// </summary>
    public static class LogConfiguration
    {
        #region Public Properties
        /// <summary>
        /// Indica se deve usar prefixos nos atributos de escopo.
        /// </summary>
        public static bool UseScopePrefix { get; set; } = true;

        /// <summary>
        /// Indica o prefixo a ser usado nos atributos de escopo, caso habilitado.
        /// </summary>
        public static string ScopePrefix { get; set; } = "Scope";

        /// <summary>
        /// Define se os logs devem incluir dados adicionais, como payloads e nomes de métodos.
        /// </summary>
        public static bool IncludeAdditionalData { get; set; } = true;

        /// <summary>
        /// Indica se os placeholders em mensagens devem ser prefixados com "MessageArgs.".
        /// </summary>
        public static bool UseMessageArgsPrefix { get; set; } = true;
        #endregion
    }
}