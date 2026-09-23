using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class JobSeekerSkillViewModel
    {
        public int SkillId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Skill")]
        public string SkillName { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        [Display(Name = "Proficiency Level")]
        public int ProficiencyLevel { get; set; } = 1;
    }
}