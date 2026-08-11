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
    }
}