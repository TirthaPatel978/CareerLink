namespace CareerLink.ViewModels
{
    public class JobMatchViewModel
    {
        public int JobId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public decimal TotalScore { get; set; }

        public decimal SkillScore { get; set; }

        public decimal EducationScore { get; set; }

        public decimal ExperienceScore { get; set; }

        public decimal LocationScore { get; set; }

        public decimal SkillWeight { get; set; }

        public decimal EducationWeight { get; set; }

        public decimal ExperienceWeight { get; set; }

        public decimal LocationWeight { get; set; }

        public int MatchedSkills { get; set; }

        public int TotalJobSkills { get; set; }

        public List<string> MatchedSkillNames { get; set; }
            = new List<string>();

        public List<string> MissingSkillNames { get; set; }
            = new List<string>();

        public string EducationExplanation { get; set; }
            = string.Empty;

        public string ExperienceExplanation { get; set; }
            = string.Empty;

        public string LocationExplanation { get; set; }
            = string.Empty;
    }
}