using CareerLink.Models.Enums;

namespace CareerLink.ViewModels
{
    public class JobDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string? CompanyDescription { get; set; }

        public string? CompanyWebsite { get; set; }

        public string? CompanyLocation { get; set; }

        public JobType JobType { get; set; }

        public string? Location { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public decimal ExperienceRequired { get; set; }

        public string? EducationRequirement { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public bool IsRemote { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<JobDetailsSkillViewModel> Skills { get; set; }
            = new List<JobDetailsSkillViewModel>();

        public bool IsSaved { get; set; }

        public bool HasApplied { get; set; }
    }

    public class JobDetailsSkillViewModel
    {
        public string Name { get; set; } = string.Empty;

        public bool IsRequired { get; set; }

        public decimal Weight { get; set; }
    }
}