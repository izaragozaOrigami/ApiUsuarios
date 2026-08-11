using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class PermissonSpecialExtended :PermissonSpecial
    {
        [JsonPropertyName("Modulo")]
        public string Modulo { get; set; }

        [JsonPropertyName("SubModulo")]
        public string SubModulo { get; set; }

        [JsonPropertyName("PermissonName")]
        public string PermissonName { get; set; }

    }
}