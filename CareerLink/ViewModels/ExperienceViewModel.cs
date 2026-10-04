using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class ExperienceViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Currently Working Here")]
        public bool IsCurrent { get; set; }

        [StringLength(2000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}