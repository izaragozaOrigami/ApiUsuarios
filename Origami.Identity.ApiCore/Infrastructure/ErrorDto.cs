using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{ 

    /// <summary>
    /// Clase de errores.
    /// </summary>
    public class ErrorDto
    {
        /// <summary>
        /// Codigo de error.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tipo de error.
        /// </summary>
        public ErrorType Type { get; set; }

        /// <summary>
        /// Mensaje de error.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Mensaje de error técnico.
        /// </summary>
        public string TechnicalMessage { get; set; }
    }
}