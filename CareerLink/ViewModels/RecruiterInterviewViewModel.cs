using CareerLink.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class RecruiterInterviewViewModel
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string ApplicantName { get; set; } = string.Empty;

        public string ApplicantEmail { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Interview Date and Time")]
        public DateTime ScheduledAt { get; set; }

        [Required]
        [Display(Name = "Interview Type")]
        public InterviewType InterviewType { get; set; }

        [Display(Name = "Meeting Link")]
        public string? MeetingLink { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        public string? Notes { get; set; }

        public InterviewStatus Status { get; set; }
            = InterviewStatus.Scheduled;
    }
}