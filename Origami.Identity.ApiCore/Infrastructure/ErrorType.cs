using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{  
    /// <summary>
    /// Enum para Tipos de Error.
    /// </summary>
    [Flags]
    public enum ErrorType
    {
        /// <summary>
        /// Indefinido.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Error Técnico.
        /// </summary>
        Technical = 1,
        /// <summary>
        /// Error de Negocio.
        /// </summary>
        Business = 2
    }
}
