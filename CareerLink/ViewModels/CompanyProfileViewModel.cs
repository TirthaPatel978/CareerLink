using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class CompanyProfileViewModel
    {
        [Required]
        [Display(Name = "Company Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Url]
        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Display(Name = "Industry")]
        public string? Industry { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Logo")]
        public string? LogoPath { get; set; }
    }
}