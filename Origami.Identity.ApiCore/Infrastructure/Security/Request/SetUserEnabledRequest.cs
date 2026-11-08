using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Request
{
    /// <summary>
    /// Habilitar o deshabilitar un usuario desde el detalle de Usuarios.
    /// </summary>
    public class SetUserEnabledRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
    }
}
