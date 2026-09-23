namespace CareerLink.Models
{
    public class JobSkill
    {
        public int JobId { get; set; }

        public int SkillId { get; set; }

        // Required or preferred skill
        public bool IsRequired { get; set; } = true;

        // Optional individual importance
        public decimal Weight { get; set; } = 1;

        public Job Job { get; set; } = null!;

        public Skill Skill { get; set; } = null!;
    }
}