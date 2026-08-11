using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{

    public class CBU
    {
        [JsonPropertyName("cbuId")]
        public string Id { get; set; }


        [JsonPropertyName("cbuName")]
        public string CBUName { get; set; }


        [JsonPropertyName("regions")]
        public List<Region> regions { get; set; }


        [JsonPropertyName("status")]
        public bool Status { get; set; }

    }
}
