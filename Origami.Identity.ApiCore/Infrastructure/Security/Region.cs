using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class Region
    {
        [JsonPropertyName("regionId")]
        public string RegionId { get; set; }

        [JsonPropertyName("regionName")]
        public string? RegionName { get; set; }

        [JsonPropertyName("sites")]
        public List<Site> Sites { get; set; }

    }
}
