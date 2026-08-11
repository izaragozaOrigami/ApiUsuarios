using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Origami.Identity.Api.Core.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public byte Level { get; set; } = 0;

        [Required]
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        [Required]
        public IList<string> Accessibility { get; set; } = new List<string>();

        [Required]
        public string NoEmployee { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? PhoneExtension { get; set; }

        public int PositionId { get; set; } = 0;

        [Required]
        public string PhotoURL { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;
    }
}