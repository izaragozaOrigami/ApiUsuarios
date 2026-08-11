using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Request
{

    public class ModuleRequest
    {
        [JsonPropertyName("moduleIdentifier")]
        public string ModuleIdentifier { get; set; }

        [Required]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [JsonPropertyName("RolName")]
        public string RolName { get; set; }

        [Required]
        [JsonPropertyName("subModules")]
        public List<SubModuleRequest> SubModules { get; set; }
    }
}
