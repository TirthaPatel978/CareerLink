using CareerLink.Models.Enums;

namespace CareerLink.ViewModels
{
    public class JobSeekerInterviewViewModel
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public DateTime ScheduledAt { get; set; }

        public InterviewType InterviewType { get; set; }

        public string? MeetingLink { get; set; }

        public string? Location { get; set; }

        public string? Notes { get; set; }

        public InterviewStatus Status { get; set; }
    }
}