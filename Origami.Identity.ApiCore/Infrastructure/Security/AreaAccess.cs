using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    /// <summary>
    /// Área de la accesibilidad del usuario. Restringe al usuario en todos sus sitios,
    /// no por sitio (dbo.AccessibilityNetUsersArea).
    /// - Al guardar o editar: WholeArea = toda el Área; si no, solo las Subareas listadas.
    /// - En el login y en el catálogo: Subareas trae siempre la lista completa permitida.
    /// </summary>
    public class AreaAccess
    {
        [JsonPropertyName("areaId")]
        public string AreaId { get; set; }

        [JsonPropertyName("areaName")]
        public string? AreaName { get; set; }

        [JsonPropertyName("wholeArea")]
        public bool WholeArea { get; set; }

        [JsonPropertyName("subareas")]
        public List<SubareaAccess> Subareas { get; set; } = new List<SubareaAccess>();
    }

    public class SubareaAccess
    {
        [JsonPropertyName("subareaId")]
        public string SubareaId { get; set; }

        [JsonPropertyName("subareaName")]
        public string? SubareaName { get; set; }
    }
}
