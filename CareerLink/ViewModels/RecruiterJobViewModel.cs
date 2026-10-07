using CareerLink.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class RecruiterJobViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Job Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Job Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Job Type")]
        public JobType JobType { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Display(Name = "Remote Job")]
        public bool IsRemote { get; set; }

        [Range(0, 999999999)]
        [Display(Name = "Minimum Salary")]
        public decimal? SalaryMin { get; set; }

        [Range(0, 999999999)]
        [Display(Name = "Maximum Salary")]
        public decimal? SalaryMax { get; set; }

        [Range(0, 100)]
        [Display(Name = "Experience Required (Years)")]
        public decimal ExperienceRequired { get; set; }

        [StringLength(200)]
        [Display(Name = "Education Requirement")]
        public string? EducationRequirement { get; set; }

        [Display(Name = "Application Deadline")]
        [DataType(DataType.Date)]
        public DateTime? ApplicationDeadline { get; set; }

        [Range(0, 100)]
        [Display(Name = "Skills Weight")]
        public decimal SkillWeight { get; set; } = 50;

        [Range(0, 100)]
        [Display(Name = "Education Weight")]
        public decimal EducationWeight { get; set; } = 20;

        [Range(0, 100)]
        [Display(Name = "Experience Weight")]
        public decimal ExperienceWeight { get; set; } = 20;

        [Range(0, 100)]
        [Display(Name = "Location Weight")]
        public decimal LocationWeight { get; set; } = 10;

        public bool IsActive { get; set; } = true;
    }
}