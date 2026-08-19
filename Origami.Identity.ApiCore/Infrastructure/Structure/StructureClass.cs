using System.Data;
using System.Reflection;

namespace Origami.Identity.Api.Core.Infrastructure
{

    public class RolesAccionAlta
    {
        public int IdModule { get; set; }
        public string NameModulo { get; set; }
        public List<Submodule> Modules { get; set; }
    }

    public class Submodule
    {
        public string NameSubmodulo { get; set; }
        public List<Views> Vistas { get; set; }
    }

    public class Views
    {
        public string ViewName { get; set; }
        public List<ActionItem> Acciones { get; set; }
    }

    /// <summary>
    /// Los cinco tipos de permiso de dbo.Permisson sobre un mismo modulo.
    /// </summary>
    /// <remarks>
    /// Van como cinco banderas y no como una lista de ids porque quien las consume es
    /// una fila de casillas en la pantalla de roles: una lista obligaria al front a
    /// traducir numeros a posiciones, y ese mapeo acabaria duplicado en los dos lados.
    ///
    /// Es informacion DISTINTA de Permisson. Aquella sale de dbo.ModuloAccesos y dice
    /// si el modulo se ve; estas salen de dbo.PermissonRoles y dicen que se puede hacer
    /// con el. Un rol puede tener lo uno sin lo otro, y de hecho ocurre.
    /// </remarks>
    public class ActionPermissons
    {
        public bool Ver { get; set; }
        public bool Editar { get; set; }
        public bool Crear { get; set; }
        public bool Solicitar { get; set; }
        public bool Autorizar { get; set; }
    }

    public class ActionItem
    {
        public string Name { get; set; }
        public int IdAction { get; set; }

        /// <summary>Visibilidad: el rol tiene fila en dbo.ModuloAccesos.</summary>
        public bool Permisson { get; set; }

        /// <summary>Que puede hacer: los tipos de dbo.PermissonRoles.</summary>
        public ActionPermissons Permisos { get; set; } = new ActionPermissons();

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Acciones que cuelgan de esta accion, a cualquier profundidad.
        /// </summary>
        /// <remarks>
        /// Aqui es donde se rompe el techo de cuatro niveles. dbo.Modulo anida mas de
        /// lo que estas cuatro clases sabian representar -- los siete bloques del Paso 2
        /// de Edicion (51..57) cuelgan de una accion, y son un quinto nivel-- y antes se
        /// perdian en silencio.
        ///
        /// Se anida sobre la MISMA clase en vez de inventar un "SubActionItem" para que
        /// no vuelva a haber un techo: un sexto o septimo nivel entra sin tocar el
        /// contrato. La consecuencia buena es que el JSON de un arbol de cuatro niveles
        /// no cambia -- la lista queda vacia -- y el FRONT que no la lea sigue
        /// funcionando igual.
        /// </remarks>
        public List<ActionItem> Acciones { get; set; } = new List<ActionItem>();
    }
}