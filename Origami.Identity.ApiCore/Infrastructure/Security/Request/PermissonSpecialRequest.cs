using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Request
{
    public class PermissonSpecialRequest
    {

        [JsonPropertyName("PermissonsSpecial")]
        public List<PermissonSpecial> PermissonsSpecial { get; set; }

    }

    public class PermissonSpecialRequest2
    {

        [JsonPropertyName("PermissonsSpecial")]
        public List<PermissonSpecial2> PermissonsSpecial { get; set; }

    }
}