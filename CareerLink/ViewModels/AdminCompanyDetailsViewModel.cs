namespace CareerLink.ViewModels
{
    public class AdminCompanyDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? Website { get; set; }

        public string? Industry { get; set; }

        public string? Location { get; set; }

        public string? LogoPath { get; set; }

        public DateTime CreatedAt { get; set; }

        public int RecruiterCount { get; set; }

        public int JobCount { get; set; }

        public int ActiveJobCount { get; set; }
    }
}