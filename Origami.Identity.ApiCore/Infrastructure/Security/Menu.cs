using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class Modulos
    {
        [JsonPropertyName("moduloId")]
        public int Idmodulo { get; set; }

        [JsonPropertyName("parentModuloId")]
        public int IdparentModulo { get; set; }

        [JsonPropertyName("titleModulo")]
        public string TitleModulo { get; set; }

        [JsonPropertyName("descriptionModulo")]
        public string DescriptionModulo { get; set; }

        [JsonPropertyName("urlModulo")]
        public string URLModulo { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("orderModulo")]
        public int OrderModulo { get; set; }

        [JsonPropertyName("subModulo")]
        public List<Modulos> subModulo { get; set; }

        [JsonPropertyName("granted")]
        public bool granted { get; set; }

    }
}
