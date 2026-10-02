using CareerLink.Models;

namespace CareerLink.ViewModels
{
    public class JobSeekerDashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;

        public string ProfessionalTitle { get; set; } = string.Empty;

        public string? Location { get; set; }

        public string? Summary { get; set; }

        public int SkillCount { get; set; }

        public int EducationCount { get; set; }

        public int ExperienceCount { get; set; }

        public int ProjectCount { get; set; }

        public int CertificationCount { get; set; }

        public int SavedJobCount { get; set; }

        public int ApplicationCount { get; set; }

        public List<JobListItemViewModel> RecentJobs { get; set; }
            = new List<JobListItemViewModel>();
    }
}