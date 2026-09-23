using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class ProjectViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Project Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(1000)]
        [Display(Name = "Technologies Used")]
        public string? Technologies { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "Project URL")]
        public string? ProjectUrl { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "GitHub URL")]
        public string? GithubUrl { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
    }
}