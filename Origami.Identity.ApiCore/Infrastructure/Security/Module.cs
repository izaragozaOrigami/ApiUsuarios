using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{

    public class Module
    {
        [JsonPropertyName("idModulo")]
        public int Idmodulo { get; set; }

        [JsonPropertyName("titleModulo")]
        public string TitleModulo { get; set; }

    }
}
