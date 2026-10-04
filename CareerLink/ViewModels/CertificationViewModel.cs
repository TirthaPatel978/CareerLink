using System.ComponentModel.DataAnnotations;

namespace CareerLink.ViewModels
{
    public class CertificationViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Certification Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Issuing Organization")]
        public string? IssuingOrganization { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Issue Date")]
        public DateTime IssueDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Url]
        [StringLength(500)]
        [Display(Name = "Credential URL")]
        public string? CredentialUrl { get; set; }
    }
}