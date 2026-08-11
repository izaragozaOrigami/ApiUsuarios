using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class SecurityUser
    {
        [JsonPropertyName("userId")]
        public string Id { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("joinDate")]
        public DateTime JoinDate { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("emailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("phoneExtension")]
        public string? PhoneExtension { get; set; }

        [JsonPropertyName("positionId")]
        public int PositionId { get; set; }

        [JsonPropertyName("noEmployee")]
        public string? NoEmployee { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("photoURL")]
        public string? PhotoURL { get; set; }

        [JsonPropertyName("roles")]
        public List<Role> Roles { get; set; }

        [JsonPropertyName("cbus")]
        public List<CBU> CBUs { get; set; }

        [JsonPropertyName("modulos")]
        public List<Modulos> Modulos { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }

    }
}