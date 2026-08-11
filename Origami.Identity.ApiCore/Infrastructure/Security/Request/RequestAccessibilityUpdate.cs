using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Request
{

    public class RequestAccessibilityUpdate
    {
        //[Required]
        public string? UserId { get; set; }

        //[Required]
        [JsonPropertyName("Accesibility")]
        public Accessibility accessibility { get; set; }
    }
}
