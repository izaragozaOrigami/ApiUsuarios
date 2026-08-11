using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Response
{

    public class GetUserAccebilityResponse
    {
        [JsonPropertyName("countGetUserAccesibility")]
        public string countGetUserAccesibility { get; set; }

        [JsonPropertyName("GetUserAccesibility")]
        public List<CBU> getUserAccesibility { get; set; }

    }
}