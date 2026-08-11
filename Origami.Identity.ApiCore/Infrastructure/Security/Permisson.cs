using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{

    public class Permisson
    {
        [JsonPropertyName("IdModulo")]
        public int IdModulo { get; set; }

        [JsonPropertyName("IdPermiso")]
        public int IdPermison { get; set; }

    }

    public class Permisson2
    {
        [JsonPropertyName("IdModulo")]
        public int IdModulo { get; set; }

    }
}