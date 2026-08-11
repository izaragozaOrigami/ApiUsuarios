using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Response
{
    public class ModulePermissonResponse
    {
        public List<ModulePermisson> modules { get; set; }

    }

    public class ModulePermissonResponse2
    {
        public List<ModulePermisson2> modules { get; set; }

    }

    public class ModulePermissonExtendedResponse
    {
        public List<PermissonSpecialExtended> permissonSpecialExtended { get; set; }

    }
}
