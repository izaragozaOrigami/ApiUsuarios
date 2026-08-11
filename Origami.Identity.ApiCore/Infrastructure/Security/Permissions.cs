using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{

    public class Permissions
    {
        [JsonPropertyName("permissionIdentifier")]
        public int PermissionIdentifier { get; set; }

        [JsonPropertyName("permissionName")]
        public string TitleModulo { get; set; }

        [JsonPropertyName("permissionRead")]
        public bool PermissionRead { get; set; }

        [JsonPropertyName("permissonEdit")]
        public bool PermissonEdit { get; set; }

        [JsonPropertyName("permissionCreate")]
        public bool PermissionCreate { get; set; }

        [JsonPropertyName("permissionAuth")]
        public bool PermissionAuth { get; set; }

        [JsonPropertyName("permissionRequest")]
        public bool PermissionRequest { get; set; }

    }
}
