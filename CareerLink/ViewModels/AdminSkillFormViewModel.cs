using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class AdminSkillFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Skill Name")]
        public string Name { get; set; } = string.Empty;
    }
}