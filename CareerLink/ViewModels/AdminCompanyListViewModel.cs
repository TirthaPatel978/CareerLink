namespace CareerLink.ViewModels
{
    public class AdminCompanyListViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Industry { get; set; }

        public string? Location { get; set; }

        public string? Website { get; set; }

        public int RecruiterCount { get; set; }

        public int JobCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}