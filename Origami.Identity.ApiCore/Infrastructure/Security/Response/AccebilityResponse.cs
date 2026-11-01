using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Response
{

    public class AccebilityResponse
    {
        [JsonPropertyName( "countAccesibility")]
        public string countAccesibility { get; set; }

        [JsonPropertyName( "accesibility")]
        public List<CBU> accesibility { get; set; }

        // Catálogo activo Área -> Subárea para asignar la restricción en la pantalla de usuarios.
        [JsonPropertyName( "areas")]
        public List<AreaAccess> areas { get; set; }

    }
}