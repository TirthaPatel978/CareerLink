namespace CareerLink.ViewModels
{
    public class AdminJobListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string JobType { get; set; } = string.Empty;
        public string? Location { get; set; }
        public bool IsRemote { get; set; }
        public bool IsActive { get; set; }
        public int ApplicationCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
    }
}