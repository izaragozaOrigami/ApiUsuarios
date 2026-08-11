using System.Data;
using System.Reflection;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class Permiso
    {
        public int IdModulo { get; set; }
        public string Titulo { get; set; }
        public int IdAccion { get; set; }
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