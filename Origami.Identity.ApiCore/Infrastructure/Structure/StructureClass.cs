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

    public class ActionItem
    {
        public string Name { get; set; }
        public int IdAction { get; set; }
        public bool Permisson { get; set; }
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