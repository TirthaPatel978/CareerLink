using Microsoft.AspNetCore.Identity;

namespace CareerLink.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public JobSeeker? JobSeeker { get; set; }

        public Recruiter? Recruiter { get; set; }

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}