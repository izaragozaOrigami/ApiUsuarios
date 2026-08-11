using System.Data;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class User
    {
        [JsonPropertyName("userId")]
        public string Id { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("joinDate")]
        public DateTime? JoinDate { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("emailConfirmed")]
        public bool? EmailConfirmed { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }


        [JsonPropertyName("NoEmployee")]
        public string? NoEmployee { get; set; }


        [JsonPropertyName("phoneExtension")]
        public string? PhoneExtension { get; set; }

        [JsonPropertyName("roles")]
        public string? Roles { get; set; }


        [JsonPropertyName("estatusId")]
        public bool? EstatusId { get; set; }


        [JsonPropertyName("photoURL")]
        public string? photoURL { get; set; }

    }

    public class UserUpdate
    {

        [JsonPropertyName("userId")]
        public string Id { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("joinDate")]
        public DateTime JoinDate { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("emailConfirmed")]
        public bool EmailConfirmed { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("NoEmployee")]
        public string NoEmployee { get; set; }

        [JsonPropertyName("phoneExtension")]
        public string? PhoneExtension { get; set; }

        [JsonPropertyName("roles")]
        public List<Role> Roles { get; set; }

        [JsonPropertyName("RolesFijos")]
        public List<Role> RolesFijos { get; set; }

        [JsonPropertyName("status")]
        public bool EstatusId { get; set; }

        [JsonPropertyName("photoURL")]
        public string? photoURL { get; set; }

        [JsonPropertyName("Position")]
        public string? Position { get; set; }
    }

    public class UserReplicate
    {

        [JsonPropertyName("userId")]
        public string Id { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("NoEmployee")]
        public string NoEmployee { get; set; }

        [JsonPropertyName("photoURL")]
        public string photoURL { get; set; }

        [JsonPropertyName("LockoutEnabled")]
        public bool lockoutEnabled { get; set; }


    }

}