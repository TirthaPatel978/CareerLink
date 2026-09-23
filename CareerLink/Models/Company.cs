namespace CareerLink.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? Website { get; set; }

        public string? Industry { get; set; }

        public string? Location { get; set; }

        public string? LogoPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Multiple recruiters can belong to one company
        public ICollection<Recruiter> Recruiters { get; set; }
            = new List<Recruiter>();

        // Jobs posted by the company
        public ICollection<Job> Jobs { get; set; }
            = new List<Job>();
    }
}