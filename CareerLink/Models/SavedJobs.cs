namespace CareerLink.Models
{
    public class SavedJob
    {
        public int JobSeekerId { get; set; }

        public int JobId { get; set; }

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        public JobSeeker JobSeeker { get; set; } = null!;

        public Job Job { get; set; } = null!;
    }
}