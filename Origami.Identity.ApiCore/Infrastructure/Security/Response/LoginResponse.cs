using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure.Security.Response
{

    public class LoginResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("fullname")]
        public string Fullname { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("email_confirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("phoneExtension")]
        public string PhoneExtension { get; set; }

        [JsonPropertyName("positionId")]
        public string PositionId { get; set; }

        [JsonPropertyName("noEmployee")]
        public string NoEmployee { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("created_at")]
        public string Created { get; set; }

        [JsonPropertyName("updated_at")]
        public string Updated { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; }

        [JsonPropertyName("timeExpirationToken")]
        public DateTime TimeExpirationToken { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("roles")]
        public List<Role> Roles { get; set; }

        [JsonPropertyName("cbus")]
        public List<CBU> CBUs { get; set; }

        [JsonPropertyName("modules")]
        public List<Modulos> Modules { get; set; }

        [JsonPropertyName("permissonAccess")]
        public PermissonAccess permissonAccess { get; set; }

    }
}