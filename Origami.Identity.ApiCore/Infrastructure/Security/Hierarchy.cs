using System.Data;
using System.Reflection;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class Permiso
    {
        public int IdModulo { get; set; }
        public string Titulo { get; set; }
        public int IdAccion { get; set; }

        /// <summary>
        /// Permisos que cuelgan de este permiso, a cualquier profundidad.
        /// </summary>
        /// <remarks>
        /// Mismo motivo que ActionItem.Acciones: el arbol de sesion tampoco sabia bajar
        /// del cuarto nivel y perdia los bloques del Paso 2 de Edicion.
        /// </remarks>
        public List<Permiso> SubPermisos { get; set; } = new List<Permiso>();
    }

    public class Pantalla
    {
        public int IdModulo { get; set; }
        public string Nombre { get; set; }
        public List<Permiso> Permisos { get; set; } = new List<Permiso>();
    }

    public class Submodulo
    {
        public int IdModulo { get; set; }
        public string Nombre { get; set; }
        public List<Pantalla> Pantallas { get; set; } = new List<Pantalla>();
    }

    public class Modulo
    {
        public int IdModulo { get; set; }
        public string Nombre { get; set; }
        public List<Submodulo> Submodulos { get; set; } = new List<Submodulo>();
    }


}