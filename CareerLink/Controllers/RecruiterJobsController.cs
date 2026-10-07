using CareerLink.Data;
using CareerLink.Models;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "Recruiter")]
    public class RecruiterJobsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecruiterJobsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /RecruiterJobs
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                TempData["WarningMessage"] =
                    "Please create your company profile before managing jobs.";

                return RedirectToAction(
                    "Profile",
                    "Company");
            }

            var jobs = await _context.Jobs
                .Where(j => j.CompanyId == companyId.Value)
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new RecruiterJobListViewModel
                {
                    Id = j.Id,
                    Title = j.Title,
                    JobType = j.JobType,
                    Location = j.Location,
                    IsRemote = j.IsRemote,
                    SalaryMin = j.SalaryMin,
                    SalaryMax = j.SalaryMax,
                    ApplicationDeadline = j.ApplicationDeadline,
                    IsActive = j.IsActive,
                    CreatedAt = j.CreatedAt,
                    ApplicationCount = j.Applications.Count
                })
                .ToListAsync();

            return View(jobs);
        }

        // GET: /RecruiterJobs/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                TempData["WarningMessage"] =
                    "Please create your company profile before creating a job.";

                return RedirectToAction(
                    "Profile",
                    "Company");
            }

            var model = new RecruiterJobViewModel
            {
                SkillWeight = 50,
                EducationWeight = 20,
                ExperienceWeight = 20,
                LocationWeight = 10,
                IsActive = true
            };

            return View(model);
        }

        // POST: /RecruiterJobs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            RecruiterJobViewModel model)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                TempData["WarningMessage"] =
                    "Please create your company profile before creating a job.";

                return RedirectToAction(
                    "Profile",
                    "Company");
            }

            ValidateJobModel(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var job = new Job
            {
                CompanyId = companyId.Value,
                Title = model.Title.Trim(),
                Description = model.Description.Trim(),
                JobType = model.JobType,
                Location = string.IsNullOrWhiteSpace(model.Location)
                    ? null
                    : model.Location.Trim(),
                IsRemote = model.IsRemote,
                SalaryMin = model.SalaryMin,
                SalaryMax = model.SalaryMax,
                ExperienceRequired = model.ExperienceRequired,
                EducationRequirement =
                    string.IsNullOrWhiteSpace(model.EducationRequirement)
                        ? null
                        : model.EducationRequirement.Trim(),
                ApplicationDeadline = model.ApplicationDeadline,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                SkillWeight = model.SkillWeight,
                EducationWeight = model.EducationWeight,
                ExperienceWeight = model.ExperienceWeight,
                LocationWeight = model.LocationWeight
            };

            _context.Jobs.Add(job);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Job has been created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /RecruiterJobs/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return RedirectToAction(
                    "Profile",
                    "Company");
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(j =>
                    j.Id == id &&
                    j.CompanyId == companyId.Value);

            if (job == null)
            {
                return NotFound();
            }

            var model = new RecruiterJobViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                JobType = job.JobType,
                Location = job.Location,
                IsRemote = job.IsRemote,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                ExperienceRequired = job.ExperienceRequired,
                EducationRequirement = job.EducationRequirement,
                ApplicationDeadline = job.ApplicationDeadline,
                SkillWeight = job.SkillWeight,
                EducationWeight = job.EducationWeight,
                ExperienceWeight = job.ExperienceWeight,
                LocationWeight = job.LocationWeight,
                IsActive = job.IsActive
            };

            return View(model);
        }

        // POST: /RecruiterJobs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            RecruiterJobViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return RedirectToAction(
                    "Profile",
                    "Company");
            }

            ValidateJobModel(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(j =>
                    j.Id == id &&
                    j.CompanyId == companyId.Value);

            if (job == null)
            {
                return NotFound();
            }

            job.Title = model.Title.Trim();
            job.Description = model.Description.Trim();
            job.JobType = model.JobType;
            job.Location = string.IsNullOrWhiteSpace(model.Location)
                ? null
                : model.Location.Trim();
            job.IsRemote = model.IsRemote;
            job.SalaryMin = model.SalaryMin;
            job.SalaryMax = model.SalaryMax;
            job.ExperienceRequired = model.ExperienceRequired;
            job.EducationRequirement =
                string.IsNullOrWhiteSpace(model.EducationRequirement)
                    ? null
                    : model.EducationRequirement.Trim();
            job.ApplicationDeadline = model.ApplicationDeadline;
            job.SkillWeight = model.SkillWeight;
            job.EducationWeight = model.EducationWeight;
            job.ExperienceWeight = model.ExperienceWeight;
            job.LocationWeight = model.LocationWeight;
            job.IsActive = model.IsActive;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Job has been updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /RecruiterJobs/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return RedirectToAction(
                    "Profile",
                    "Company");
            }

            var job = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j =>
                    j.Id == id &&
                    j.CompanyId == companyId.Value);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: /RecruiterJobs/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var companyId = await GetRecruiterCompanyIdAsync();

            if (!companyId.HasValue)
            {
                return RedirectToAction(
                    "Profile",
                    "Company");
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(j =>
                    j.Id == id &&
                    j.CompanyId == companyId.Value);

            if (job == null)
            {
                return NotFound();
            }

            job.IsActive = !job.IsActive;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = job.IsActive
                ? "Job has been reopened successfully."
                : "Job has been closed successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<int?> GetRecruiterCompanyIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return null;
            }

            var recruiter = await _context.Recruiters
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.ApplicationUserId == user.Id);

            return recruiter?.CompanyId;
        }

        private void ValidateJobModel(
            RecruiterJobViewModel model)
        {
            if (model.SalaryMin.HasValue &&
                model.SalaryMax.HasValue &&
                model.SalaryMin > model.SalaryMax)
            {
                ModelState.AddModelError(
                    nameof(model.SalaryMax),
                    "Maximum salary must be greater than or equal to minimum salary.");
            }

            var totalWeight =
                model.SkillWeight +
                model.EducationWeight +
                model.ExperienceWeight +
                model.LocationWeight;

            if (totalWeight != 100)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Matching weights must add up to exactly 100%.");
            }

            if (model.ApplicationDeadline.HasValue &&
                model.ApplicationDeadline.Value.Date < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError(
                    nameof(model.ApplicationDeadline),
                    "Application deadline cannot be in the past.");
            }
        }
    }
}