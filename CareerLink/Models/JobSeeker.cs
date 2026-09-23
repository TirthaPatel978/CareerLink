using static System.Net.Mime.MediaTypeNames;

namespace CareerLink.Models
{
    public class JobSeeker
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;

        public string ProfessionalTitle { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Relationship with ApplicationUser
        public ApplicationUser ApplicationUser { get; set; } = null!;

        // Skills
        public ICollection<JobSeekerSkill> Skills { get; set; }
            = new List<JobSeekerSkill>();

        // Education
        public ICollection<Education> Educations { get; set; }
            = new List<Education>();

        // Work experience
        public ICollection<Experience> Experiences { get; set; }
            = new List<Experience>();

        // Projects
        public ICollection<Project> Projects { get; set; }
            = new List<Project>();

        // Certifications
        public ICollection<Certification> Certifications { get; set; }
            = new List<Certification>();

        // Applications
        public ICollection<Application> Applications { get; set; }
            = new List<Application>();

        // Saved jobs
        public ICollection<SavedJob> SavedJobs { get; set; }
            = new List<SavedJob>();
    }
}