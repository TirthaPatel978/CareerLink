using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class JobSeekerProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Professional Title")]
        public string ProfessionalTitle { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Professional Summary")]
        public string Summary { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [StringLength(200)]
        [Display(Name = "Location")]
        public string? Location { get; set; }
    }
}