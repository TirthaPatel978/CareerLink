using CareerLink.Models.Enums;

namespace CareerLink.ViewModels
{
    public class RecruiterJobListViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public JobType JobType { get; set; }

        public string? Location { get; set; }

        public bool IsRemote { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public int ApplicationCount { get; set; }
    }
}