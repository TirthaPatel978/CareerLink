namespace CareerLink.Models
{
    public class Education
    {
        public int Id { get; set; }

        public int JobSeekerId { get; set; }

        public string Degree { get; set; } = string.Empty;

        public string FieldOfStudy { get; set; } = string.Empty;

        public string Institution { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Grade { get; set; }

        public JobSeeker JobSeeker { get; set; } = null!;
    }
}