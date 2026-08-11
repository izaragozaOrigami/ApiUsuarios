using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Request
{
    public class SubModuleRequest
    {
        [JsonPropertyName("submoduleIdentifier")]
        public int SubmoduleIdentifier { get; set; }

        [JsonPropertyName("submoduleName")]
        public string SubmoduleName { get; set; }


        [JsonPropertyName("submoduleDescription")]
        public string Description { get; set; }

        [Required]
        [JsonPropertyName("permissions")]
        public List<Permissions> Permissions { get; set; }
    }
}
