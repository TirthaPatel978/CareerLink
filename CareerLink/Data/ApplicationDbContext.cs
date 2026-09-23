using CareerLink.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<JobSeeker> JobSeekers { get; set; }

        public DbSet<Recruiter> Recruiters { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobSeekerSkill> JobSeekerSkills { get; set; }

        public DbSet<JobSkill> JobSkills { get; set; }

        public DbSet<Education> Educations { get; set; }

        public DbSet<Experience> Experiences { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Certification> Certifications { get; set; }

        public DbSet<Application> Applications { get; set; }

        public DbSet<Interview> Interviews { get; set; }

        public DbSet<SavedJob> SavedJobs { get; set; }

        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // JobSeekerSkill composite primary key
            modelBuilder.Entity<JobSeekerSkill>()
                .HasKey(x => new
                {
                    x.JobSeekerId,
                    x.SkillId
                });

            // JobSkill composite primary key
            modelBuilder.Entity<JobSkill>()
                .HasKey(x => new
                {
                    x.JobId,
                    x.SkillId
                });

            // SavedJob composite primary key
            modelBuilder.Entity<SavedJob>()
                .HasKey(x => new
                {
                    x.JobSeekerId,
                    x.JobId
                });


            // ApplicationUser -> JobSeeker
            modelBuilder.Entity<JobSeeker>()
                .HasOne(x => x.ApplicationUser)
                .WithOne(x => x.JobSeeker)
                .HasForeignKey<JobSeeker>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);


            // ApplicationUser -> Recruiter
            modelBuilder.Entity<Recruiter>()
                .HasOne(x => x.ApplicationUser)
                .WithOne(x => x.Recruiter)
                .HasForeignKey<Recruiter>(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Company -> Recruiters
            modelBuilder.Entity<Recruiter>()
                .HasOne(x => x.Company)
                .WithMany(x => x.Recruiters)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.SetNull);


            // Company -> Jobs
            modelBuilder.Entity<Job>()
                .HasOne(x => x.Company)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);


            // JobSeeker -> Education
            modelBuilder.Entity<Education>()
                .HasOne(x => x.JobSeeker)
                .WithMany(x => x.Educations)
                .HasForeignKey(x => x.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);


            // JobSeeker -> Experience
            modelBuilder.Entity<Experience>()
                .HasOne(x => x.JobSeeker)
                .WithMany(x => x.Experiences)
                .HasForeignKey(x => x.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);


            // JobSeeker -> Projects
            modelBuilder.Entity<Project>()
                .HasOne(x => x.JobSeeker)
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);


            // JobSeeker -> Certifications
            modelBuilder.Entity<Certification>()
                .HasOne(x => x.JobSeeker)
                .WithMany(x => x.Certifications)
                .HasForeignKey(x => x.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);


            // JobSeekerSkill -> JobSeeker
            modelBuilder.Entity<JobSeekerSkill>()
                .HasOne(x => x.JobSeeker)
                .WithMany(x => x.Skills)
                .HasForeignKey(x => x.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);


            // JobSeekerSkill -> Skill
            modelBuilder.Entity<JobSeekerSkill>()
                .HasOne(x => x.Skill)
                .WithMany(x => x.JobSeekerSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);


            // JobSkill -> Job
            modelBuilder.Entity<JobSkill>()
                .HasOne(x => x.Job)
                .WithMany(x => x.Skills)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);


            // JobSkill -> Skill
            modelBuilder.Entity<JobSkill>()
                .HasOne(x => x.Skill)
                .WithMany(x => x.JobSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);


            // Application -> Job
            modelBuilder.Entity<Application>()
                .HasOne(x => x.Job)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);


            // Application -> JobSeeker
            modelBuilder.Entity<Application>()
                .HasOne(x => x.JobSeeker)
                .WithMany(x => x.Applications)
                .HasForeignKey(x => x.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);


            // Application -> Interviews
            modelBuilder.Entity<Interview>()
                .HasOne(x => x.Application)
                .WithMany(x => x.Interviews)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);


            // SavedJob -> JobSeeker
            modelBuilder.Entity<SavedJob>()
                .HasOne(x => x.JobSeeker)
                .WithMany(x => x.SavedJobs)
                .HasForeignKey(x => x.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);


            // SavedJob -> Job
            modelBuilder.Entity<SavedJob>()
                .HasOne(x => x.Job)
                .WithMany(x => x.SavedJobs)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);


            // ApplicationUser -> Notifications
            modelBuilder.Entity<Notification>()
                .HasOne(x => x.ApplicationUser)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Decimal precision
            modelBuilder.Entity<Job>()
                .Property(x => x.SalaryMin)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Job>()
                .Property(x => x.SalaryMax)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Job>()
                .Property(x => x.ExperienceRequired)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Job>()
                .Property(x => x.SkillWeight)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Job>()
                .Property(x => x.EducationWeight)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Job>()
                .Property(x => x.ExperienceWeight)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Job>()
                .Property(x => x.LocationWeight)
                .HasPrecision(5, 2);

            modelBuilder.Entity<JobSkill>()
                .Property(x => x.Weight)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Application>()
                .Property(x => x.MatchScore)
                .HasPrecision(5, 2);
        }
    }
}