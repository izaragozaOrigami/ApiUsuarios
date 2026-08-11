using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class CreateUserBindingModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "NoEmployee")]
        public string NoEmployee { get; set; }

        [Required]
        [Display(Name = "PositionId")]
        public int PositionId { get; set; }

        [Display(Name = "PhoneNumber")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "PhoneExtension")]
        public string? PhoneExtension { get; set; }

        [Display(Name = "Status")]
        [JsonIgnore]
        public string? Estatus { get; set; }

       // [Required]
        [Display(Name = "URLPhotos")]
        public string? URLPhotos { get; set; }


       // [Required]
        [Display(Name = "Roles")]
        public List<Role> Roles { get; set; }


       // [Required]
        [Display(Name = "RolesFijos")]
        public List<Role> RolesFijos { get; set; }


      //  [Required]
        [Display(Name = "Accesibility")]
        public Accessibility Accessibility { get; set; }

        [Required]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0} debe ser por lo menos {2} .", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "El password y la confirmacion no coinciden.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordBindingModel
    {
       
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    
    }

    public class ResetPasswordBindingModel
    {
        [Required]
        [Display(Name = "Recovery Code")]
        [JsonPropertyName("recoveryCode")]
        public string RecoveryCode { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [JsonPropertyName("confirmPassword")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class UpdateUserBindingModel
    {

        [Required]
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }


    public class UpdateUserModel
    {

        [Required]
        [Display(Name = "Id")]
        public string id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string phoneNumber { get; set; }

        [Required]
        [Display(Name = "Roles")]
        public List<Role> roles { get; set; }
    }


    public class RequestComponent
    {
        [Required]
        [Display(Name = "components")]
        public List<ComponentRequest> Components { get; set; }

     
    }
}