using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{ 
    /// <summary>
    /// Clase base de respuesta.
    /// </summary>
    public class ResponseBaseDto
    {
        public ResponseBaseDto() { Success = false; }
        /// <summary>
        /// Hace referencia a la bandera que indica si el servicio se ejecuto de manera correcta.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///     Descripción breve de resultado obtenido.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Codigo
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Hace referencia a la lista de errores.
        /// </summary>
        public List<ErrorDto> ErrorList { get; set; }

        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
    }
}
