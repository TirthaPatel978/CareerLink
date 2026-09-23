using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class EducationViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Degree")]
        public string Degree { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Field of Study")]
        public string FieldOfStudy { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Institution")]
        public string Institution { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [StringLength(50)]
        [Display(Name = "Grade")]
        public string? Grade { get; set; }
    }
}