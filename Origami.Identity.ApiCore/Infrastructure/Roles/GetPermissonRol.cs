using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure

{
    public class GetPermissonRol
    {
        [JsonPropertyName("RoleId")]
        public string RoleId { get; set; }
    }


    public class GetPermissonByRoles
    {
        [JsonPropertyName("RoleId")]
        public List<string> RoleId { get; set; }
    }

}