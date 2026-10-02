using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CareerLink.ViewModels
{
    public class ResumeViewModel
    {
        public string? ExistingFileName { get; set; }

        public bool HasResume { get; set; }

        [Display(Name = "Resume")]
        public IFormFile? ResumeFile { get; set; }
    }
}