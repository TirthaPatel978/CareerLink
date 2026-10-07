using CareerLink.Models.Enums;

namespace CareerLink.ViewModels
{
    public class RecruiterApplicantListViewModel
    {
        public int ApplicationId { get; set; }

        public int JobId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string ApplicantName { get; set; } = string.Empty;

        public string ApplicantEmail { get; set; } = string.Empty;

        public string ProfessionalTitle { get; set; } = string.Empty;

        public string? Location { get; set; }

        public decimal? MatchScore { get; set; }

        public ApplicationStatus Status { get; set; }

        public DateTime AppliedAt { get; set; }
    }
}