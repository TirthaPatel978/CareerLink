namespace CareerLink.Models
{
    public class Recruiter
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; } = string.Empty;

        public int? CompanyId { get; set; }

        public string? Phone { get; set; }

        public string? JobTitle { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationship with ApplicationUser
        public ApplicationUser ApplicationUser { get; set; } = null!;

        // Relationship with Company
        public Company? Company { get; set; }
    }
}