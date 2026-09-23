namespace CareerLink.Models
{
    public class JobSeekerSkill
    {
        public int JobSeekerId { get; set; }

        public int SkillId { get; set; }

        // Optional proficiency level from 1 to 5
        public int ProficiencyLevel { get; set; } = 1;

        public JobSeeker JobSeeker { get; set; } = null!;

        public Skill Skill { get; set; } = null!;
    }
}