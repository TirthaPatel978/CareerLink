using CareerLink.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class JobSearchViewModel
    {
        [Display(Name = "Search")]
        public string? Search { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Job Type")]
        public JobType? JobType { get; set; }

        [Display(Name = "Remote Only")]
        public bool RemoteOnly { get; set; }

        public List<JobListItemViewModel> Jobs { get; set; }
            = new List<JobListItemViewModel>();
    }

    public class JobListItemViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public JobType JobType { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public bool IsRemote { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}