using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class Accessibility
    {

        //[Required]
        [JsonPropertyName("CBUS")]
        public List<CBU> CBU { get; set; }


    }
}
