using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class RoleResponse
    {
        [JsonPropertyName( "countRole")]
        public string CountRol { get; set; }

        [JsonPropertyName( "roles")]
        public List<Role> roles { get; set; }

    }
}