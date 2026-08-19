using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{

    public class Permisson
    {
        [JsonPropertyName("IdModulo")]
        public int IdModulo { get; set; }

        [JsonPropertyName("IdPermiso")]
        public int IdPermison { get; set; }

    }

    public class Permisson2
    {
        [JsonPropertyName("IdModulo")]
        public int IdModulo { get; set; }

        /// <summary>
        /// Tipo de permiso sobre ese modulo, segun dbo.Permisson:
        /// 1 VER, 2 EDITAR, 3 CREAR, 4 SOLICITAR, 5 AUTORIZAR.
        /// </summary>
        /// <remarks>
        /// Antes esta clase solo traia IdModulo, asi que la pantalla de roles no podia
        /// expresar QUE se puede hacer con el modulo -- solo si se ve. El efecto era
        /// grave y silencioso: guardar un rol borraba dbo.PermissonRoles entero para ese
        /// rol (Usp_Security_PermissonRoles_DEL limpia las dos tablas) y despues solo
        /// repoblaba dbo.ModuloAccesos, dejando al rol sin VER, EDITAR, CREAR, SOLICITAR
        /// ni AUTORIZAR. Un rol que pasara por la pantalla perdia el permiso que el
        /// motor de solicitudes valida.
        ///
        /// Se manda una entrada por PAR (modulo, permiso), que es como esta la tabla.
        /// Un cliente viejo que no lo mande deja 0 y se ignora al guardar, en vez de
        /// escribir una fila con un tipo que no existe en el catalogo.
        /// </remarks>
        [JsonPropertyName("IdPermiso")]
        public int IdPermiso { get; set; }
    }
}