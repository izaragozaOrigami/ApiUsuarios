using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{


    /// <summary>
    /// Clase para los mensajes de error de datos
    /// </summary>
    public static class ErrorMessage
    {
        /// <summary>
        /// Mensaje de resultado de datos
        /// </summary>
        public const string DATA_RESULT_NULL = "No se encontró ningún registro";

        /// <summary>
        /// Mensaje de conexión de base de datos
        /// </summary>
        public const string DATA_RESULT = "Error al ejecutar la petición en base de datos";

        /// <summary>
        /// Error al ejecutarse el SP
        /// </summary>
        public const string SP_EXECUTE_ERROR = "El Stored Procedure: \"{0}\" se ejecutó con errores";

        /// <summary>
        /// Error al insertar el bulk
        /// </summary>
        public const string BULK_INSERT_TABLE = "Error al insertar el bulk \"{0}\"";

        /// <summary>
        /// Error de la actualización del bulk
        /// </summary>
        public const string BULK_UPDATE_TABLE = "Error al actualizar el bulk \"{0}\"";

        /// <summary>
        /// Código de mensaje error en capa de base de datos
        /// </summary>
        public const string DATA_ERRORCODE = "500";
    }
}