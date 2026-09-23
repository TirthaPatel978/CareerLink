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
    }
}