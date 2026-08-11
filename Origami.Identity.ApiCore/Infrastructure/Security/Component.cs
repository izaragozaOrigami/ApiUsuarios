using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class Component
    {
        [JsonPropertyName("componentId")]
        public string IdComponent { get; set; }

        [JsonPropertyName("componentParentId")]
        public string IdComponentParent { get; set; }

        [JsonPropertyName("typeComponentId")]
        public int? IdTipoComponent { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("idPermisson")]
        public int? IdPermisson { get; set; }

        [JsonPropertyName("subComponent")]
        public List<Component> subComponent { get; set; }

    }


    public class ComponentRequest
    {
        [JsonPropertyName("componentId")]
        public string IdComponent { get; set; }

        [JsonPropertyName("moduloId")]
        public int ModuloId { get; set; }

    }
}
