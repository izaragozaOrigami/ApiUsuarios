using System.Data;
using System.Reflection;

namespace Origami.Identity.Api.Core.Infrastructure
{

    public class RecoveryCodes
    {
        public string RecoveryCode { get; set; }
        public string Email { get; set; }
        public DateTime ValidityDate { get; set; }
    }
}