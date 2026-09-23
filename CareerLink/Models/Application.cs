using CareerLink.Models.Enums;

namespace CareerLink.Models
{
    public class Application
    {
        public int Id { get; set; }

        public int JobId { get; set; }

        public int JobSeekerId { get; set; }

        // Optional uploaded resume
        public string? ResumePath { get; set; }

        public string? CoverLetter { get; set; }

        // Result of the rule-based matching system
        public decimal? MatchScore { get; set; }

        public ApplicationStatus Status { get; set; }
            = ApplicationStatus.Applied;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Job Job { get; set; } = null!;

        public JobSeeker JobSeeker { get; set; } = null!;

        public ICollection<Interview> Interviews { get; set; }
            = new List<Interview>();
    }
}