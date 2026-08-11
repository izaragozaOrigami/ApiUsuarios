using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Request
{

    public class UsersRequest
    {
        [JsonPropertyName("page")]
        public string page { get; set; }

        [JsonPropertyName("pageLength")]
        public int pageLength { get; set; }

    }
}