using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Request
{

    public class RecoveryCodeEmailRequest
    {
        [JsonPropertyName("email")]
        public string? email { get; set; }
    }
}