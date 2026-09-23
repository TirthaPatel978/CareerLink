namespace CareerLink.Models
{
    public class Project
    {
        public int Id { get; set; }

        public int JobSeekerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Technologies { get; set; }

        public string? ProjectUrl { get; set; }

        public string? GithubUrl { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public JobSeeker JobSeeker { get; set; } = null!;
    }
}