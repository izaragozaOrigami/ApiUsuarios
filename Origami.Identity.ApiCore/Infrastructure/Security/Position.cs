using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class Position
    {
        [JsonPropertyName("PositionId")]
        public int Id { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("EstatusId")]
        public bool EstatusId { get; set; }

    }
}