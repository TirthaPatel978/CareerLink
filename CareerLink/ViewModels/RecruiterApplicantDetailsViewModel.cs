using CareerLink.Models.Enums;

namespace CareerLink.ViewModels
{
    public class RecruiterApplicantDetailsViewModel
    {
        public int ApplicationId { get; set; }

        public int JobId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string ApplicantName { get; set; } = string.Empty;

        public string ApplicantEmail { get; set; } = string.Empty;

        public string? ApplicantPhone { get; set; }

        public string ProfessionalTitle { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public string? Location { get; set; }

        public decimal? MatchScore { get; set; }

        public ApplicationStatus Status { get; set; }

        public DateTime AppliedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? ResumePath { get; set; }

        public string? CoverLetter { get; set; }
    }
}