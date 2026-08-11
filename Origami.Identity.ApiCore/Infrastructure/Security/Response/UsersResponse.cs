using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Response
{

    public class UsersResponse
    {
        [JsonPropertyName( "countUsers")]
        public string CountUsers { get; set; }

        [JsonPropertyName("users")]
        public List<User> Users { get; set; }

    }
}