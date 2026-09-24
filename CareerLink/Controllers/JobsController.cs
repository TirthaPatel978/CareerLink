using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Jobs
        [HttpGet]
        public async Task<IActionResult> Index(
            JobSearchViewModel model)
        {
            var query = _context.Jobs
                .Include(x => x.Company)
                .Where(x => x.IsActive);

            // Search by job title, description, or company name.
            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                var search = model.Search.Trim();

                query = query.Where(x =>
                    x.Title.Contains(search) ||
                    x.Description.Contains(search) ||
                    x.Company.Name.Contains(search));
            }

            // Filter by location.
            if (!string.IsNullOrWhiteSpace(model.Location))
            {
                var location = model.Location.Trim();

                query = query.Where(x =>
                    x.Location != null &&
                    x.Location.Contains(location));
            }

            // Filter by job type.
            if (model.JobType.HasValue)
            {
                query = query.Where(x =>
                    x.JobType == model.JobType.Value);
            }

            // Filter remote jobs.
            if (model.RemoteOnly)
            {
                query = query.Where(x => x.IsRemote);
            }

            // Do not display jobs whose application deadline
            // has already passed.
            var today = DateTime.UtcNow.Date;

            query = query.Where(x =>
                !x.ApplicationDeadline.HasValue ||
                x.ApplicationDeadline.Value.Date >= today);

            model.Jobs = await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new JobListItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    CompanyName = x.Company.Name,
                    Location = x.Location,
                    JobType = x.JobType,
                    SalaryMin = x.SalaryMin,
                    SalaryMax = x.SalaryMax,
                    IsRemote = x.IsRemote,
                    ApplicationDeadline = x.ApplicationDeadline,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            return View(model);
        }

        // GET: /Jobs/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var job = await _context.Jobs
                .Include(x => x.Company)
                .Include(x => x.Skills)
                    .ThenInclude(x => x.Skill)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.IsActive);

            if (job == null)
            {
                return NotFound();
            }

            var isSaved = await _context.SavedJobs
                .AnyAsync(x =>
                    x.JobId == id &&
                    x.JobSeekerId == jobSeeker.Id);

            var hasApplied = await _context.Applications
                .AnyAsync(x =>
                    x.JobId == id &&
                    x.JobSeekerId == jobSeeker.Id);

            var model = new JobDetailsViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                CompanyName = job.Company.Name,
                CompanyDescription = job.Company.Description,
                CompanyWebsite = job.Company.Website,
                CompanyLocation = job.Company.Location,
                JobType = job.JobType,
                Location = job.Location,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                ExperienceRequired = job.ExperienceRequired,
                EducationRequirement = job.EducationRequirement,
                ApplicationDeadline = job.ApplicationDeadline,
                IsRemote = job.IsRemote,
                CreatedAt = job.CreatedAt,
                IsSaved = isSaved,
                HasApplied = hasApplied,

                Skills = job.Skills
                    .OrderByDescending(x => x.IsRequired)
                    .ThenBy(x => x.Skill.Name)
                    .Select(x => new JobDetailsSkillViewModel
                    {
                        Name = x.Skill.Name,
                        IsRequired = x.IsRequired,
                        Weight = x.Weight
                    })
                    .ToList()
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Match(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var jobSeeker = await _context.JobSeekers
                .Include(x => x.Skills)
                    .ThenInclude(x => x.Skill)
                .Include(x => x.Educations)
                .Include(x => x.Experiences)
                .FirstOrDefaultAsync(x =>
                    x.ApplicationUserId == user.Id);

            if (jobSeeker == null)
            {
                return NotFound(
                    "Job Seeker profile was not found.");
            }

            var job = await _context.Jobs
                .Include(x => x.Company)
                .Include(x => x.Skills)
                    .ThenInclude(x => x.Skill)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.IsActive);

            if (job == null)
            {
                return NotFound("Job was not found.");
            }

            // -----------------------------
            // 1. Skills
            // -----------------------------

            var jobSkills = job.Skills
                .ToList();

            var seekerSkillIds = jobSeeker.Skills
                .Select(x => x.SkillId)
                .ToHashSet();

            var matchedSkills = jobSkills
                .Where(x => seekerSkillIds.Contains(x.SkillId))
                .ToList();

            var missingSkills = jobSkills
                .Where(x => !seekerSkillIds.Contains(x.SkillId))
                .ToList();

            decimal skillScore = 0;

            if (jobSkills.Count > 0)
            {
                var totalSkillWeight = jobSkills
                    .Sum(x => x.Weight);

                if (totalSkillWeight > 0)
                {
                    var matchedSkillWeight = matchedSkills
                        .Sum(x => x.Weight);

                    skillScore =
                        (matchedSkillWeight / totalSkillWeight)
                        * job.SkillWeight;
                }
            }
            else
            {
                skillScore = job.SkillWeight;
            }

            // -----------------------------
            // 2. Education
            // -----------------------------

            decimal educationScore = 0;

            string educationExplanation;

            if (string.IsNullOrWhiteSpace(
                job.EducationRequirement))
            {
                educationScore = job.EducationWeight;

                educationExplanation =
                    "No specific education requirement was provided.";
            }
            else
            {
                var requirement =
                    job.EducationRequirement.Trim();

                var educationMatch =
                    jobSeeker.Educations.Any(x =>
                        x.Degree.Contains(
                            requirement,
                            StringComparison.OrdinalIgnoreCase) ||
                        x.FieldOfStudy.Contains(
                            requirement,
                            StringComparison.OrdinalIgnoreCase));

                if (educationMatch)
                {
                    educationScore = job.EducationWeight;

                    educationExplanation =
                        "Your education matches the job requirement.";
                }
                else
                {
                    educationScore = 0;

                    educationExplanation =
                        "No matching education was found for the stated requirement.";
                }
            }

            // -----------------------------
            // 3. Experience
            // -----------------------------

            decimal totalExperienceYears = 0;

            foreach (var experience in jobSeeker.Experiences)
            {
                var endDate = experience.IsCurrent
                    ? DateTime.UtcNow
                    : experience.EndDate ?? DateTime.UtcNow;

                if (endDate > experience.StartDate)
                {
                    totalExperienceYears +=
                        (decimal)(
                            endDate - experience.StartDate)
                            .TotalDays / 365.25m;
                }
            }

            decimal experienceScore;

            if (job.ExperienceRequired <= 0)
            {
                experienceScore = job.ExperienceWeight;
            }
            else
            {
                var experienceRatio =
                    totalExperienceYears /
                    job.ExperienceRequired;

                experienceRatio =
                    Math.Min(1m, Math.Max(0m, experienceRatio));

                experienceScore =
                    experienceRatio *
                    job.ExperienceWeight;
            }

            string experienceExplanation;

            if (job.ExperienceRequired <= 0)
            {
                experienceExplanation =
                    "No minimum experience requirement was specified.";
            }
            else
            {
                experienceExplanation =
                    $"You have approximately " +
                    $"{totalExperienceYears:0.0} years of experience " +
                    $"against a requirement of " +
                    $"{job.ExperienceRequired:0.0} years.";
            }

            // -----------------------------
            // 4. Location
            // -----------------------------

            decimal locationScore;

            string locationExplanation;

            if (job.IsRemote)
            {
                locationScore = job.LocationWeight;

                locationExplanation =
                    "This is a remote job, so the location requirement is satisfied.";
            }
            else if (
                string.IsNullOrWhiteSpace(job.Location) ||
                string.IsNullOrWhiteSpace(jobSeeker.Location))
            {
                locationScore = 0;

                locationExplanation =
                    "Location information is not available for both the job and your profile.";
            }
            else
            {
                var jobLocation =
                    job.Location.Trim();

                var seekerLocation =
                    jobSeeker.Location.Trim();

                if (jobLocation.Contains(
                        seekerLocation,
                        StringComparison.OrdinalIgnoreCase) ||
                    seekerLocation.Contains(
                        jobLocation,
                        StringComparison.OrdinalIgnoreCase))
                {
                    locationScore = job.LocationWeight;

                    locationExplanation =
                        "Your profile location matches the job location.";
                }
                else
                {
                    locationScore = 0;

                    locationExplanation =
                        $"Your profile location ({seekerLocation}) " +
                        $"does not match the job location ({jobLocation}).";
                }
            }

            // -----------------------------
            // 5. Total
            // -----------------------------

            var totalScore =
                skillScore +
                educationScore +
                experienceScore +
                locationScore;

            totalScore =
                Math.Min(100m, Math.Max(0m, totalScore));

            var model = new JobMatchViewModel
            {
                JobId = job.Id,
                JobTitle = job.Title,
                CompanyName = job.Company.Name,

                TotalScore = Math.Round(totalScore, 2),

                SkillScore = Math.Round(skillScore, 2),
                EducationScore = Math.Round(educationScore, 2),
                ExperienceScore = Math.Round(experienceScore, 2),
                LocationScore = Math.Round(locationScore, 2),

                SkillWeight = job.SkillWeight,
                EducationWeight = job.EducationWeight,
                ExperienceWeight = job.ExperienceWeight,
                LocationWeight = job.LocationWeight,

                MatchedSkills = matchedSkills.Count,
                TotalJobSkills = jobSkills.Count,

                MatchedSkillNames = matchedSkills
                    .Select(x => x.Skill.Name)
                    .OrderBy(x => x)
                    .ToList(),

                MissingSkillNames = missingSkills
                    .Select(x => x.Skill.Name)
                    .OrderBy(x => x)
                    .ToList(),

                EducationExplanation = educationExplanation,

                ExperienceExplanation = experienceExplanation,

                LocationExplanation = locationExplanation
            };

            return View(model);
        }
    }
}