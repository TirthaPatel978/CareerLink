using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class RecruiterProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        [Display(Name = "Job Title")]
        public string? JobTitle { get; set; }

        [Display(Name = "Company")]
        public string? CompanyName { get; set; }

        public int? CompanyId { get; set; }
    }
}