using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace AntiFraud.System.Application.Utilities
{
    /// <summary>
    /// Classe de extensão contendo métodos para objetos.
    /// </summary>
    public static class ObjectExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Cria uma instância do tipo especificado.
        /// </summary>
        /// <typeparam name="T">O tipo da instância a ser criada. Deve ser uma classe.</typeparam>
        /// <returns>
        /// Uma instância do tipo especificado, ou null se não for possível criar a instância.
        /// </returns>
        /// <remarks>
        /// O método tenta criar uma instância do tipo <typeparamref name="T"/> usando várias abordagens:
        /// <list type="number">
        /// <item>
        /// <description>Usa <see cref="Activator.CreateInstance(Type, BindingFlags, Binder, object[], CultureInfo)"/> com <see cref="BindingFlags.NonPublic"/>, <see cref="BindingFlags.Public"/> e <see cref="BindingFlags.Instance"/>.</description>
        /// </item>
        /// <item>
        /// <description>Tenta encontrar um construtor sem parâmetros e invocá-lo.</description>
        /// </item>
        /// <item>
        /// <description>Tenta encontrar qualquer construtor e invocá-lo com valores padrão para os parâmetros.</description>
        /// </item>
        /// </list>
        /// Se todas as tentativas falharem, o método retorna null.
        /// </remarks>
        public static T? CreateInstance<T>()
            where T : class
            => (T?)CreateInstance(typeof(T));

        /// <summary>
        /// Cria uma instância do tipo especificado.
        /// </summary>
        /// <param name="type">O tipo da instância a ser criada.</param>
        /// <returns>
        /// Uma instância do tipo especificado, ou null se não for possível criar a instância.
        /// </returns>
        /// <remarks>
        /// O método tenta criar uma instância do tipo <paramref name="type"/> usando várias abordagens:
        /// <list type="number">
        /// <item>
        /// <description>Usa <see cref="Activator.CreateInstance(Type, BindingFlags, Binder, object[], CultureInfo)"/> com <see cref="BindingFlags.NonPublic"/>, <see cref="BindingFlags.Public"/> e <see cref="BindingFlags.Instance"/>.</description>
        /// </item>
        /// <item>
        /// <description>Tenta encontrar um construtor sem parâmetros e invocá-lo.</description>
        /// </item>
        /// <item>
        /// <description>Tenta encontrar qualquer construtor e invocá-lo com valores padrão para os parâmetros.</description>
        /// </item>
        /// </list>
        /// Se todas as tentativas falharem, o método retorna null.
        /// </remarks>
        public static object? CreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, CultureInfo.InvariantCulture);
            }
            catch
            {
                var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);
                if (constructor.IsNotNull())
                    return constructor.Invoke(null);

                try
                {
                    constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).FirstOrDefault();
                    if (constructor is null)
                        return null;

                    var parameters = constructor.GetParameters();
                    var parameterValues = new object?[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                        parameterValues[i] = parameters[i].ParameterType.IsValueType
                            ? CreateInstance(parameters[i].ParameterType)
                            : default;

                    return constructor.Invoke(parameterValues);

                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Verifica se o objeto é nulo.
        /// </summary>
        /// <param name="obj">O objeto a ser verificado.</param>
        /// <returns>True se o objeto não for nulo, caso contrário, false.</returns>
        /// <remarks>
        /// O atributo <see cref="NotNullWhenAttribute"/> informa ao compilador que o argumento usado para o parâmetro obj é não nulo quando o método retorna true.
        /// Quando o método retorna false, o argumento tem o mesmo estado nulo que tinha antes do método ser chamado.
        /// </remarks>
        public static bool IsNull([NotNullWhen(false)] this object? obj) => obj is null;

        /// <summary>
        /// Verifica se o objeto não é nulo.
        /// </summary>
        /// <param name="obj">O objeto a ser verificado.</param>
        /// <returns>True se o objeto não for nulo, caso contrário, false.</returns>
        /// <remarks>
        /// O atributo <see cref="NotNullWhenAttribute"/> informa ao compilador que o argumento usado para o parâmetro obj é não nulo quando o método retorna true.
        /// Quando o método retorna false, o argumento tem o mesmo estado nulo que tinha antes do método ser chamado.
        /// </remarks>
        public static bool IsNotNull([NotNullWhen(true)] this object? obj) => !obj.IsNull();

        /// <summary>
        /// Verifica se o objeto é nulo ou vazio.
        /// </summary>
        /// <param name="obj">O objeto a ser verificado.</param>
        /// <returns>True se o objeto for nulo ou vazio, caso contrário, false.</returns>
        /// <remarks>
        /// O atributo <see cref="NotNullWhenAttribute"/> informa ao compilador que o argumento usado para o parâmetro obj é não nulo quando o método retorna true.
        /// Quando o método retorna false, o argumento tem o mesmo estado nulo que tinha antes do método ser chamado.
        /// </remarks>
        public static bool IsNullOrEmpty([NotNullWhen(false)] this object? obj)
        {
            switch (obj)
            {
                case null:
                case Guid guid when guid == Guid.Empty:
                case string s when string.IsNullOrWhiteSpace(s):
                case ICollection { Count: 0 }:
                case Array { Length: 0 }:
                case IEnumerable e when !e.GetEnumerator().MoveNext():
                    return true;
                default: return false;
            }
        }

        /// <summary>
        /// Verifica se o objeto não é nulo ou vazio.
        /// </summary>
        /// <param name="obj">O objeto a ser verificado.</param>
        /// <returns>True se o objeto não for nulo ou vazio, caso contrário, false.</returns>
        /// <remarks>
        /// O atributo <see cref="NotNullWhenAttribute"/> informa ao compilador que o argumento usado para o parâmetro obj é não nulo quando o método retorna true.
        /// Quando o método retorna false, o argumento tem o mesmo estado nulo que tinha antes do método ser chamado.
        /// </remarks>
        public static bool IsNotNullOrEmpty([NotNullWhen(true)] this object? obj)
            => !obj.IsNullOrEmpty();

        /// <summary>
        /// Verifica se o objeto do tipo especificado é anulável.
        /// </summary>
        /// <typeparam name="T">O tipo do objeto a ser verificado.</typeparam>
        /// <param name="obj">O objeto a ser verificado.</param>
        /// <returns>
        /// <c>true</c> se o objeto for anulável; caso contrário, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Este método verifica se um objeto é anulável, considerando os seguintes casos:
        /// - Se o objeto em si é <c>null</c>, ele é considerado anulável.
        /// - Todos os tipos de referência são anuláveis.
        /// - Tipos de valor são anuláveis se forem do tipo <see cref="Nullable{T}"/>.
        /// </remarks>
        public static bool IsNullable<T>(this T obj)
        {
            if (obj is null)
                return true;

            Type type = typeof(T);
            if (!type.IsValueType)
                return true;

            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        /// Obtém o valor de uma propriedade específica de um objeto.
        /// </summary>
        /// <param name="obj">O objeto do qual obter o valor da propriedade.</param>
        /// <param name="propertyName">O nome da propriedade desejada.</param>
        /// <returns>O valor da propriedade ou null se a propriedade não existir.</returns>
        public static object? GetValue(this object? obj, string propertyName)
            => obj?.GetValue<object>(propertyName);

        /// <summary>
        /// Obtém o valor de uma propriedade específica de um objeto e converte para o tipo fornecido.
        /// </summary>
        /// <typeparam name="T">O tipo desejado para a propriedade.</typeparam>
        /// <param name="obj">O objeto do qual obter o valor da propriedade.</param>
        /// <param name="propertyName">O nome da propriedade desejada.</param>
        /// <returns>O valor da propriedade convertido para o tipo T ou null se a propriedade não existir.</returns>
        public static T? GetValue<T>(this object? obj, string propertyName)
            => (T?)obj?.GetType().GetProperty(propertyName)?.GetValue(obj);
        #endregion
    }
}