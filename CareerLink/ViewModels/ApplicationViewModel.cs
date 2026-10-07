using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CareerLink.ViewModels
{
    public class ApplicationViewModel
    {
        public int JobId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string? JobLocation { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        [Display(Name = "Cover Letter")]
        [StringLength(
            5000,
            ErrorMessage = "Cover letter cannot exceed 5000 characters.")]
        public string? CoverLetter { get; set; }

        [Display(Name = "Resume")]
        public IFormFile? Resume { get; set; }

        public string? ExistingResumePath { get; set; }
    }
}