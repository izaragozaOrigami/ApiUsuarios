using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class PermissonSpecial
    {
        [JsonPropertyName("UserId")]
        public string UserId { get; set; }

        [JsonPropertyName("IdModulo")]
        public int IdModulo { get; set; }

        [JsonPropertyName("IdPermisson")]
        public int IdPermisson { get; set; }

        [JsonPropertyName("InitialDate")]
        public DateTime InitialDate { get; set; }

        [JsonPropertyName("FinalDate")]
        public DateTime FinalDate { get; set; }

        [JsonPropertyName("Status")]
        public bool Status { get; set; }

    }

    public class PermissonSpecial2
    {
        [JsonPropertyName("UserId")]
        public string UserId { get; set; }

        [JsonPropertyName("IdModulo")]
        public int IdModulo { get; set; }

        [JsonPropertyName("IdPermisson")]
        public int IdPermisson { get; set; }

        [JsonPropertyName("InitialDate")]
        public DateTime InitialDate { get; set; }

        [JsonPropertyName("FinalDate")]
        public DateTime FinalDate { get; set; }

        [JsonPropertyName("Status")]
        public bool Status { get; set; }

    }
}