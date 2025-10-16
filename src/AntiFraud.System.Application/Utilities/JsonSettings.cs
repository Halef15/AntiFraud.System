using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AntiFraud.System.Application.Utilities
{
    /// <summary>
    /// Configurações JSON pré-definidas.
    /// </summary>
    public static class JsonSettings
    {
        #region Static Variables
        /// <summary>
        /// Configuração para CamelCase.
        /// </summary>
        public static readonly JsonSerializerSettings CamelCaseSerializerSettings = new()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        /// <summary>
        /// Configuração para API REST.
        /// </summary>
        public static readonly JsonSerializerSettings RestSerializerSettings = new()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = [new StringEnumConverter()],
        };
   
        #endregion
    }
}