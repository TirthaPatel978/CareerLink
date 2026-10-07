using CareerLink.Data;
using CareerLink.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerLink.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminJobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminJobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var jobs = await _context.Jobs
                .AsNoTracking()
                .Include(x => x.Company)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AdminJobListViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    CompanyName = x.Company.Name,
                    JobType = x.JobType.ToString(),
                    Location = x.Location,
                    IsRemote = x.IsRemote,
                    IsActive = x.IsActive,
                    ApplicationCount = x.Applications.Count,
                    CreatedAt = x.CreatedAt,
                    ApplicationDeadline = x.ApplicationDeadline
                })
                .ToListAsync();

            return View(jobs);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.Jobs
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new AdminJobDetailsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    CompanyName = x.Company.Name,
                    JobType = x.JobType.ToString(),
                    Location = x.Location,
                    IsRemote = x.IsRemote,
                    SalaryMin = x.SalaryMin,
                    SalaryMax = x.SalaryMax,
                    ExperienceRequired = x.ExperienceRequired,
                    EducationRequirement = x.EducationRequirement,
                    ApplicationDeadline = x.ApplicationDeadline,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    ApplicationCount = x.Applications.Count
                })
                .FirstOrDefaultAsync();

            if (job == null)
            {
                return NotFound("Job was not found.");
            }

            return View(job);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(x => x.Id == id);

            if (job == null)
            {
                return NotFound("Job was not found.");
            }

            job.IsActive = !job.IsActive;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = job.IsActive
                ? "Job has been activated."
                : "Job has been closed.";

            return RedirectToAction(nameof(Index));
        }
    }
}