using CareerLink.Models.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace CareerLink.Models
{
    public class Job
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public JobType JobType { get; set; }

        public string? Location { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        // Required years of experience
        public decimal ExperienceRequired { get; set; }

        // Example: Bachelor's Degree in Computer Science
        public string? EducationRequirement { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public bool IsRemote { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Matching weights
        public decimal SkillWeight { get; set; } = 50;

        public decimal EducationWeight { get; set; } = 20;

        public decimal ExperienceWeight { get; set; } = 20;

        public decimal LocationWeight { get; set; } = 10;

        // Company
        public Company Company { get; set; } = null!;

        // Required/preferred skills
        public ICollection<JobSkill> Skills { get; set; }
            = new List<JobSkill>();

        // Applications
        public ICollection<Application> Applications { get; set; }
            = new List<Application>();

        // Saved jobs
        public ICollection<SavedJob> SavedJobs { get; set; }
            = new List<SavedJob>();
    }
}