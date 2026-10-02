using CareerLink.Models.Enums;

namespace CareerLink.ViewModels
{
    public class HomeViewModel
    {
        public List<FeaturedJobViewModel> FeaturedJobs { get; set; }
            = new List<FeaturedJobViewModel>();
    }

    public class FeaturedJobViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string? Location { get; set; }

        public bool IsRemote { get; set; }

        public JobType JobType { get; set; }

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public List<string> Skills { get; set; }
            = new List<string>();
    }
}