using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Response
{

    public class PositionResponse
    {
        [JsonPropertyName( "countPosition")]
        public string countPosition { get; set; }

        [JsonPropertyName("Positions")]
        public List<Position> positions { get; set; }

    }
}