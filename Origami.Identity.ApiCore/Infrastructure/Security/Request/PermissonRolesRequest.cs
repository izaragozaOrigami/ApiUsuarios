using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Request
{


    public class PermissonRolesRequest
    {
        [JsonPropertyName("RolName")]
        public string RolName { get; set; }

        [JsonPropertyName("Permissons")]
        public List<Permisson> Permissons { get; set; }

        [JsonPropertyName("RolId")]
        public string RolId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class PermissonRolesRequest2
    {
        [JsonPropertyName("RolName")]
        public string? RolName { get; set; }

        [JsonPropertyName("Permissons")]
        public List<Permisson2> Permissons { get; set; }

        [JsonPropertyName("RolId")]
        public string? RolId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}