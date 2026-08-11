using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class Site
    {
        [JsonPropertyName("siteByRegionId")]
        public string? SiteByRegionId { get; set; }

        [JsonPropertyName("siteId")]
        public string? Id { get; set; }

        [JsonPropertyName("cedisName")]
        public string? CedisName { get; set; }

    }
}