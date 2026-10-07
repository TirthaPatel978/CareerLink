using CareerLink.Models.Enums;

namespace CareerLink.ViewModels
{
    public class AdminReportsViewModel
    {
        // Users
        public int TotalUsers { get; set; }
        public int JobSeekers { get; set; }
        public int Recruiters { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }

        // Companies and Jobs
        public int TotalCompanies { get; set; }
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int ClosedJobs { get; set; }

        // Applications
        public int TotalApplications { get; set; }
        public int AppliedApplications { get; set; }
        public int UnderReviewApplications { get; set; }
        public int ShortlistedApplications { get; set; }
        public int InterviewApplications { get; set; }
        public int SelectedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int WithdrawnApplications { get; set; }

        // Interviews
        public int TotalInterviews { get; set; }
        public int ScheduledInterviews { get; set; }
        public int CompletedInterviews { get; set; }
        public int CancelledInterviews { get; set; }
        public int RescheduledInterviews { get; set; }
    }
}