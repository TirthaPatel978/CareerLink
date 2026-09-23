namespace CareerLink.Models
{
    public class Skill
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Job seekers who have this skill
        public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; }
            = new List<JobSeekerSkill>();

        // Jobs requiring this skill
        public ICollection<JobSkill> JobSkills { get; set; }
            = new List<JobSkill>();
    }
}