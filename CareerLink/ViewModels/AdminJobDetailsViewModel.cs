namespace CareerLink.ViewModels
{
    public class AdminJobDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool IsRemote { get; set; }

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }

        public decimal ExperienceRequired { get; set; }
        public string? EducationRequirement { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int ApplicationCount { get; set; }
    }
}