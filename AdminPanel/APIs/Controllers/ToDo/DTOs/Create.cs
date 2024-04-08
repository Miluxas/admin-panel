using System;
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.APIs.Controllers.ToDo.DTOs
{
    public record CreateRequestBodyDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Title { get; set; } = String.Empty;

        [Required]
        public double Effort { get; set; } 
    }
}

