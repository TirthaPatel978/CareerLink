using CareerLink.Models.Enums;

namespace CareerLink.Models
{
    public class Interview
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public DateTime ScheduledAt { get; set; }

        public InterviewType InterviewType { get; set; }

        public string? MeetingLink { get; set; }

        public string? Location { get; set; }

        public string? Notes { get; set; }

        public InterviewStatus Status { get; set; }
            = InterviewStatus.Scheduled;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Application Application { get; set; } = null!;
    }
}