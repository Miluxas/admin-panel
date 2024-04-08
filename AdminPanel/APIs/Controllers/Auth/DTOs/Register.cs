using System;
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.APIs.Controllers.Auth.DTOs
{
    public record RegisterRequestBodyDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Email { get; set; } = String.Empty;

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; } = String.Empty;

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Password { get; set; } = String.Empty;
    }
}

