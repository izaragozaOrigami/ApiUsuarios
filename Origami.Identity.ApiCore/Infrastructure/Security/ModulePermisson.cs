using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{

    public class ModulePermisson
    {
        [JsonPropertyName( "idModulo")]
        public int Idmodulo { get; set; }

        [JsonPropertyName( "modulo")]
        public string Modulo { get; set; }

        [JsonPropertyName( "titleModulo")]
        public string TitleModulo { get; set; }

        [JsonPropertyName( "permissonName")]
        public string PermissonName { get; set; }


        [JsonPropertyName( "idVer")]
        public int idVer { get; set; }

        [JsonPropertyName( "ver")]
        public int Ver { get; set; }

        [JsonPropertyName( "idEditar")]
        public int IdEditar { get; set; }

        [JsonPropertyName( "editar")]
        public int Editar { get; set; }

        [JsonPropertyName( "idCrear")]
        public int idCrear { get; set; }

        [JsonPropertyName( "crear")]
        public int Crear { get; set; }

        [JsonPropertyName( "idSolicitar")]
        public int idSolicitar { get; set; }

        [JsonPropertyName( "Solicitar")]
        public int Solicitar { get; set; }

        [JsonPropertyName( "idAutorizar")]
        public int idAutorizar { get; set; }

        [JsonPropertyName( "autorizar")]
        public int Autorizar { get; set; }

        [JsonPropertyName( "idBorrar")]
        public int idBorrar { get; set; }

        [JsonPropertyName( "Borrar")]
        public int Borrar { get; set; }

        [JsonPropertyName( "modulePermissons")]
        public List<ModulePermisson> modulePermissons { get; set; }

    }

    public class ModulePermisson2
    {
        [JsonPropertyName( "idModulo")]
        public int Idmodulo { get; set; }

        [JsonPropertyName( "modulo")]
        public string Modulo { get; set; }

        [JsonPropertyName( "titleModulo")]
        public string TitleModulo { get; set; }

        [JsonPropertyName( "permissonName")]
        public string PermissonName { get; set; }

        [JsonPropertyName( "modulePermissons")]
        public List<ModulePermisson2> modulePermissons { get; set; }

    }
}
