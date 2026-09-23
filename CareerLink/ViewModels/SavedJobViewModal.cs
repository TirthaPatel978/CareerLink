using CareerLink.Models.Enums;

namespace CareerLink.ViewModels
{
    public class SavedJobViewModel
    {
        public int JobId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public JobType JobType { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public bool IsRemote { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public DateTime SavedAt { get; set; }
    }
}