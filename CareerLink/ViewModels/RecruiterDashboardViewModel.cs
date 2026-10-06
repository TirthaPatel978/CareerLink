namespace CareerLink.ViewModels
{
    public class RecruiterDashboardViewModel
    {
        public string RecruiterName { get; set; } = string.Empty;

        public string? CompanyName { get; set; }

        public int TotalJobs { get; set; }

        public int ActiveJobs { get; set; }

        public int ClosedJobs { get; set; }

        public int TotalApplications { get; set; }
    }
}