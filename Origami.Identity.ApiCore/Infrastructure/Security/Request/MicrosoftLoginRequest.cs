using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Request
{
    /// <summary>
    /// Inicio de sesión con Microsoft: el front manda el id_token que le entregó Microsoft Entra ID.
    /// </summary>
    public class MicrosoftLoginRequest
    {
        [JsonPropertyName("idToken")]
        public string? IdToken { get; set; }
    }
}
